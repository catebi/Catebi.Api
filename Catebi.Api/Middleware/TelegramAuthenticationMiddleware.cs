using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;
using Catebi.Api.Domain.Features.AdoptionBot;
using Catebi.Api.Domain.Features.AdoptionBot.Enums;
using Catebi.Api.Models.Telegram;

namespace Catebi.Api.Middleware;

public class TelegramAuthenticationMiddleware(
    RequestDelegate next,
    IConfiguration configuration,
    ILogger<TelegramAuthenticationMiddleware> logger,
    IServiceProvider serviceProvider)
{
    private readonly RequestDelegate _next = next;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<TelegramAuthenticationMiddleware> _logger = logger;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip authentication for health checks, swagger, etc.
        if (ShouldSkipAuthentication(context.Request.Path))
        {
            await _next(context);
            return;
        }

        // Skip authentication for controllers that don't start with AdoptionBot
        var path = context.Request.Path.Value ?? "";
        if (!path.StartsWith("/AdoptionBot", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("tma ", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Missing or invalid Authorization header for path: {Path}", context.Request.Path);
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized: Missing or invalid authorization header");
            return;
        }

        var initData = authHeader.Substring(4);

        if (!ValidateTelegramWebAppData(initData, out var telegramUser))
        {
            _logger.LogWarning("Invalid Telegram authorization data");
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized: Invalid Telegram authorization");
            return;
        }

        // Create claims for the user
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, telegramUser.Id.ToString()),
            new(ClaimTypes.Name, telegramUser.Username ?? telegramUser.FirstName),
            new("TelegramId", telegramUser.Id.ToString()),
            new("FirstName", telegramUser.FirstName),
            new("LastName", telegramUser.LastName ?? ""),
            new("Username", telegramUser.Username ?? ""),
            new("LanguageCode", telegramUser.LanguageCode ?? ""),
            new("IsPremium", (telegramUser.IsPremium ?? false).ToString()),
            new("PhotoUrl", telegramUser.PhotoUrl ?? ""),
            new("AllowsWriteToPm", (telegramUser.AllowsWriteToPm ?? false).ToString())
        };

        // Look up user in Airtable to get their status and role
        using (var scope = _serviceProvider.CreateScope())
        {
            var userService = scope.ServiceProvider.GetRequiredService<IAdoptionBotUserService>();
            try
            {
                var user = await userService.FindUserByTelegramId(telegramUser.Id);
                if (user != null)
                {
                    // Add all user data as claims for easy access in CurrentUserService
                    claims.Add(new Claim("UserRecordId", user.RecordId ?? ""));
                    claims.Add(new Claim("UserName", user.Name ?? ""));
                    claims.Add(new Claim("UserTelegram", user.Telegram ?? ""));
                    claims.Add(new Claim("UserStatus", user.Status ?? ""));
                    claims.Add(new Claim("UserLanguage", user.Language ?? ""));
                    claims.Add(new Claim("IsVolunteer", (user.IsVolunteer ?? false).ToString()));
                    claims.Add(new Claim("UsePayedAccount", (user.UsePayedAccount ?? false).ToString()));
                    claims.Add(new Claim("AdditionalContact", user.AdditionalContact ?? ""));
                    claims.Add(new Claim("UserNotes", user.Notes ?? ""));

                    // Add role claim for authorization
                    if (!string.IsNullOrEmpty(user.Role))
                    {
                        claims.Add(new Claim(ClaimTypes.Role, user.Role));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error looking up user in Airtable");
            }
        }

        var identity = new ClaimsIdentity(claims, "Telegram");
        var principal = new ClaimsPrincipal(identity);

        context.User = principal;

        await _next(context);
    }

    private static bool ShouldSkipAuthentication(PathString path)
    {
        var skipPaths = new[] { "/health", "/swagger", "/api/test", "/api/auth" };
        return skipPaths.Any(skipPath => path.StartsWithSegments(skipPath, StringComparison.OrdinalIgnoreCase));
    }

    private bool ValidateTelegramWebAppData(string initData, out TelegramUser? user)
    {
        user = null;

        try
        {
            var data = HttpUtility.ParseQueryString(initData);
            var receivedHash = data["hash"];

            if (string.IsNullOrEmpty(receivedHash))
            {
                _logger.LogWarning("Missing hash in initData");
                return false;
            }

            // Remove hash from data for validation
            data.Remove("hash");

            // Check auth_date to prevent replay attacks
            var authDateStr = data["auth_date"];
            if (!string.IsNullOrEmpty(authDateStr) && long.TryParse(authDateStr, out var authDate))
            {
                var authDateTime = DateTimeOffset.FromUnixTimeSeconds(authDate);
                var timeDifference = DateTimeOffset.UtcNow - authDateTime;

                // Reject if data is older than 1 hour
                if (timeDifference.TotalHours > 1)
                {
                    _logger.LogWarning("InitData is too old: {TimeDifference} hours", timeDifference.TotalHours);
                    return false;
                }
            }

            // Sort and create data-check-string
            var dataCheckString = string.Join("\n",
                data.AllKeys
                    .Where(k => k != null)
                    .OrderBy(k => k)
                    .Select(k => $"{k}={data[k]}"));

            // Validate hash
            var botToken = _configuration["AdoptionBot:Telegram:Token"];
            if (string.IsNullOrEmpty(botToken))
            {
                _logger.LogError("Bot token not configured");
                return false;
            }

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes("WebAppData"));
            var secretKey = hmac.ComputeHash(Encoding.UTF8.GetBytes(botToken));

            using var hmac2 = new HMACSHA256(secretKey);
            var hashBytes = hmac2.ComputeHash(Encoding.UTF8.GetBytes(dataCheckString));
            var calculatedHash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            if (calculatedHash != receivedHash)
            {
                _logger.LogWarning("Invalid Telegram hash. Expected: {Expected}, Received: {Received}",
                    calculatedHash, receivedHash);
                return false;
            }

            // Parse user data
            var userJson = data["user"];
            if (!string.IsNullOrEmpty(userJson))
            {
                user = JsonSerializer.Deserialize<TelegramUser>(userJson);
                return user != null;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating Telegram data");
            return false;
        }
    }
}
