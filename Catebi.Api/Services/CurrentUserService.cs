using System.Security.Claims;
using Catebi.Api.Models.Telegram;
using Catebi.Api.Domain.Features.AdoptionBot;
using Catebi.Api.Domain.Features.AdoptionBot.Enums;
using Catebi.Api.Extensions;

namespace Catebi.Api.Services;

/// <summary>
/// Service for accessing the current authenticated Telegram user
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CurrentUserService> _logger;

    private CurrentTelegramUser? _cachedUser;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor,
        IServiceProvider serviceProvider,
        ILogger<CurrentUserService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// Gets the current authenticated user. Returns null if not authenticated.
    /// </summary>
    public async Task<CurrentTelegramUser?> GetCurrentUserAsync()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        // Return cached user if available
        if (_cachedUser != null)
        {
            return _cachedUser;
        }

        try
        {
            var telegramUser = await BuildCurrentUserFromClaimsAsync(user);
            _cachedUser = telegramUser;
            return telegramUser;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error building current user from claims");
            return null;
        }
    }

    /// <summary>
    /// Gets the current authenticated user. Throws exception if not authenticated.
    /// </summary>
    public async Task<CurrentTelegramUser> GetRequiredCurrentUserAsync()
    {
        var user = await GetCurrentUserAsync();
        if (user == null)
        {
            throw new UnauthorizedAccessException("User is not authenticated");
        }
        return user;
    }

    /// <summary>
    /// Gets the Telegram ID of the current user. Returns null if not authenticated.
    /// </summary>
    public long? GetCurrentTelegramId()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        var telegramIdClaim = user?.FindFirst("TelegramId")?.Value;

        if (string.IsNullOrEmpty(telegramIdClaim) || !long.TryParse(telegramIdClaim, out var telegramId))
        {
            return null;
        }

        return telegramId;
    }

    /// <summary>
    /// Checks if the current user is authenticated
    /// </summary>
    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;

    /// <summary>
    /// Checks if the current user is an admin
    /// </summary>
    public bool IsAdmin => _httpContextAccessor.HttpContext?.User?.IsAdmin() == true;

    private async Task<CurrentTelegramUser?> BuildCurrentUserFromClaimsAsync(ClaimsPrincipal claimsPrincipal)
    {
        var telegramIdClaim = claimsPrincipal.FindFirst("TelegramId")?.Value;
        if (string.IsNullOrEmpty(telegramIdClaim) || !long.TryParse(telegramIdClaim, out var telegramId))
        {
            return null;
        }

        var currentUser = new CurrentTelegramUser
        {
            // Telegram properties from claims
            TelegramId = telegramId,
            FirstName = claimsPrincipal.FindFirst("FirstName")?.Value ?? string.Empty,
            LastName = claimsPrincipal.FindFirst("LastName")?.Value,
            Username = claimsPrincipal.FindFirst("Username")?.Value,
            LanguageCode = claimsPrincipal.FindFirst("LanguageCode")?.Value,
            IsPremium = claimsPrincipal.FindFirst("IsPremium")?.Value == "True",
            PhotoUrl = claimsPrincipal.FindFirst("PhotoUrl")?.Value,
            AllowsWriteToPm = claimsPrincipal.FindFirst("AllowsWriteToPm")?.Value == "True",

            // Application properties from claims (if available)
            RecordId = claimsPrincipal.FindFirst("UserRecordId")?.Value,
            Name = claimsPrincipal.FindFirst("UserName")?.Value,
            Telegram = claimsPrincipal.FindFirst("UserTelegram")?.Value,
            Status = claimsPrincipal.FindFirst("UserStatus")?.Value,
            Role = TryParseUserRole(claimsPrincipal.FindFirst(ClaimTypes.Role)?.Value),
            Language = claimsPrincipal.FindFirst("UserLanguage")?.Value,
            IsVolunteer = claimsPrincipal.FindFirst("IsVolunteer")?.Value == "True",
            UsePayedAccount = claimsPrincipal.FindFirst("UsePayedAccount")?.Value == "True",
            AdditionalContact = claimsPrincipal.FindFirst("AdditionalContact")?.Value,
            Notes = claimsPrincipal.FindFirst("UserNotes")?.Value
        };

        // If we don't have user data in claims, try to fetch from database
        if (string.IsNullOrEmpty(currentUser.RecordId))
        {
            await TryLoadUserDataFromDatabaseAsync(currentUser);
        }

        return currentUser;
    }

    private async Task TryLoadUserDataFromDatabaseAsync(CurrentTelegramUser currentUser)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var userService = scope.ServiceProvider.GetService<IAdoptionBotUserService>();

            if (userService != null)
            {
                var dbUser = await userService.FindUserByTelegramId(currentUser.TelegramId);
                if (dbUser != null)
                {
                    // Map database user data to current user
                    currentUser.RecordId = dbUser.RecordId;
                    currentUser.Name = dbUser.Name;
                    currentUser.Telegram = dbUser.Telegram;
                    currentUser.Status = dbUser.Status;
                    currentUser.Role = TryParseUserRole(dbUser.Role);
                    currentUser.Language = dbUser.Language;
                    currentUser.IsVolunteer = dbUser.IsVolunteer ?? false;
                    currentUser.UsePayedAccount = dbUser.UsePayedAccount ?? false;
                    currentUser.AdditionalContact = dbUser.AdditionalContact;
                    currentUser.Notes = dbUser.Notes;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load user data from database for Telegram ID {TelegramId}", currentUser.TelegramId);
            // Don't throw - we can work with just the Telegram data
        }
    }

    private static UserRoles? TryParseUserRole(string? roleString)
    {
        if (string.IsNullOrEmpty(roleString))
            return null;

        if (Enum.TryParse<UserRoles>(roleString, ignoreCase: true, out var role))
            return role;

        return null;
    }
}
