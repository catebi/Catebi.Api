using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Cryptography;
using System.Text;

namespace Catebi.Api.Authorization;

/// <summary>
/// Attribute to validate Tribute webhook signature using HMAC-SHA256
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class TributeWebhookAuthorizeAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<TributeWebhookAuthorizeAttribute>>();

        // Get the Tribute API key from configuration
        var apiKey = configuration["Finance:Tribute:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
        {
            logger.LogError("Tribute API key not configured");
            context.Result = new UnauthorizedObjectResult(new { error = "Tribute API key not configured" });
            return;
        }

        // Get the signature from the header
        if (!context.HttpContext.Request.Headers.TryGetValue("trbt-signature", out var signatureHeader))
        {
            logger.LogWarning("Missing trbt-signature header");
            context.Result = new UnauthorizedObjectResult(new { error = "Missing signature header" });
            return;
        }

        var receivedSignature = signatureHeader.ToString();

        // Read the request body
        context.HttpContext.Request.EnableBuffering();
        using var reader = new StreamReader(
            context.HttpContext.Request.Body,
            encoding: Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false,
            bufferSize: 1024,
            leaveOpen: true);
        
        var body = await reader.ReadToEndAsync();
        context.HttpContext.Request.Body.Position = 0;

        if (string.IsNullOrEmpty(body))
        {
            logger.LogWarning("Empty request body");
            context.Result = new BadRequestObjectResult(new { error = "Empty request body" });
            return;
        }

        // Calculate HMAC-SHA256 signature
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(apiKey));
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(body));
        var calculatedSignature = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

        // Compare signatures
        if (!calculatedSignature.Equals(receivedSignature, StringComparison.OrdinalIgnoreCase))
        {
            logger.LogWarning($"Invalid signature. Expected: {calculatedSignature}, Received: {receivedSignature}");
            context.Result = new UnauthorizedObjectResult(new { error = "Invalid signature" });
            return;
        }

        logger.LogInformation("Tribute webhook signature validated successfully");
        await next();
    }
}

