using System.Security.Cryptography;
using System.Text;

namespace Catebi.Api.Middleware;

public class TributeWebhookAuthenticationMiddleware(
    RequestDelegate next,
    ILogger<TributeWebhookAuthenticationMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<TributeWebhookAuthenticationMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
    {
        // Only process Tribute webhook endpoints
        var path = context.Request.Path.Value ?? "";
        if (!path.StartsWith("/Tribute/Webhook", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        // Get the Tribute API key from configuration
        var apiKey = configuration["Finance:Tribute:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
        {
            _logger.LogError("Tribute API key not configured");
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"error\":\"Tribute API key not configured\"}");
            return;
        }

        // Get the signature from the header
        if (!context.Request.Headers.TryGetValue("trbt-signature", out var signatureHeader))
        {
            _logger.LogWarning("Missing trbt-signature header");
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"error\":\"Missing signature header\"}");
            return;
        }

        var receivedSignature = signatureHeader.ToString();

        // Enable buffering so the body can be read multiple times
        context.Request.EnableBuffering();

        // Read the request body
        using var reader = new StreamReader(
            context.Request.Body,
            encoding: Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false,
            bufferSize: 1024,
            leaveOpen: true);

        var body = await reader.ReadToEndAsync();

        // Reset the stream position for the next middleware/controller
        context.Request.Body.Position = 0;

        if (string.IsNullOrEmpty(body))
        {
            _logger.LogWarning("Empty request body");
            context.Response.StatusCode = 400;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"error\":\"Empty request body\"}");
            return;
        }

        _logger.LogInformation($"Request body length: {body.Length}");
        _logger.LogDebug($"Request body content: {body}");

        // Calculate HMAC-SHA256 signature
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(apiKey));
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(body));
        var calculatedSignature = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

        // Compare signatures
        if (!calculatedSignature.Equals(receivedSignature, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning($"Invalid signature. Expected: {calculatedSignature}, Received: {receivedSignature}");
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"error\":\"Invalid signature\"}");
            return;
        }

        _logger.LogInformation("Tribute webhook signature validated successfully");
        _logger.LogDebug($"Stream position before controller: {context.Request.Body.Position}");
        _logger.LogDebug($"Stream can seek: {context.Request.Body.CanSeek}");

        // Continue to the next middleware/controller
        await _next(context);
    }
}

