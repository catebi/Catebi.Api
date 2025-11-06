using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Catebi.Api.Authorization;
using Catebi.Api.Domain.Features.Finance.Tribute;
using Catebi.Api.Domain.Features.Finance.Tribute.Models;
using System.Text.Json;

namespace Catebi.Api.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class TributeController(ITributeService tributeService, ILogger<TributeController> logger) : ControllerBase
{
    private readonly ITributeService _tributeService = tributeService;
    private readonly ILogger<TributeController> _logger = logger;

    /// <summary>
    /// Webhook endpoint for new subscription events from Tribute
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> WebhookNewSubscription([FromBody] TributeWebhookRequest request)
    {
        try
        {
            _logger.LogInformation($"Processing webhook: {request?.Name ?? "NULL"} (CreatedAt: {request?.CreatedAt}, SentAt: {request?.SentAt})");

            if (request == null)
            {
                _logger.LogError("Request is null");
                return BadRequest(new { error = "Invalid request" });
            }

            // Deserialize the payload
            string payloadJson;
            try
            {
                payloadJson = request.Payload.GetRawText();
                _logger.LogDebug($"Payload JSON: {payloadJson}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calling GetRawText. Payload.ValueKind: {request.Payload.ValueKind}");
                throw;
            }

            var payload = JsonSerializer.Deserialize<NewSubscriptionPayload>(
                payloadJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (payload == null)
            {
                _logger.LogError("Failed to deserialize subscription payload");
                return BadRequest(new { error = "Invalid payload format" });
            }

            _logger.LogInformation($"Parsed subscription - Name: '{payload.SubscriptionName}', Amount: {payload.Amount / 100.0:F2} {payload.Currency}, Period: {payload.Period}, TelegramUserId: {payload.TelegramUserId}");

            // Process the subscription with webhook name
            var subscriptionDto = await _tributeService.ProcessNewSubscription(
                request.Name,
                payload,
                request.CreatedAt,
                request.SentAt);

            _logger.LogInformation($"Successfully processed subscription - RecordId: {subscriptionDto.RecordId}, SubscriptionId: {payload.SubscriptionId}, User: {payload.TelegramUserId}");

            return Ok(new
            {
                success = true,
                message = "Subscription processed successfully",
                recordId = subscriptionDto.RecordId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing new subscription webhook");
            return StatusCode(500, new { error = "Internal server error processing subscription" });
        }
    }

    /// <summary>
    /// Webhook endpoint for recurrent donation events from Tribute
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> WebhookRecurrentDonation([FromBody] TributeWebhookRequest request)
    {
        try
        {
            _logger.LogInformation($"Processing webhook: {request?.Name ?? "NULL"} (CreatedAt: {request?.CreatedAt}, SentAt: {request?.SentAt})");

            if (request == null)
            {
                _logger.LogError("Request is null");
                return BadRequest(new { error = "Invalid request" });
            }

            // Deserialize the payload
            string payloadJson;
            try
            {
                payloadJson = request.Payload.GetRawText();
                _logger.LogDebug($"Payload JSON: {payloadJson}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calling GetRawText. Payload.ValueKind: {request.Payload.ValueKind}");
                throw;
            }

            var payload = JsonSerializer.Deserialize<RecurrentDonationPayload>(
                payloadJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (payload == null)
            {
                _logger.LogError("Failed to deserialize donation payload");
                return BadRequest(new { error = "Invalid payload format" });
            }

            _logger.LogInformation($"Parsed donation - Name: '{payload.DonationName}', Amount: {payload.Amount / 100.0:F2} {payload.Currency}, Period: {payload.Period}, TelegramUserId: {payload.TelegramUserId}, Anonymous: {payload.Anonymously}");

            // Process the donation with webhook name
            var donationDto = await _tributeService.ProcessRecurrentDonation(
                request.Name,
                payload,
                request.CreatedAt,
                request.SentAt);

            _logger.LogInformation($"Successfully processed donation - RecordId: {donationDto.RecordId}, DonationRequestId: {payload.DonationRequestId}, User: {payload.TelegramUserId}");

            return Ok(new
            {
                success = true,
                message = "Donation processed successfully",
                recordId = donationDto.RecordId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing recurrent donation webhook");
            return StatusCode(500, new { error = "Internal server error processing donation" });
        }
    }

    /// <summary>
    /// Get all subscriptions (Admin endpoint)
    /// </summary>
    [HttpGet]
    [TelegramAuthorize]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> GetSubscriptions()
    {
        try
        {
            var subscriptions = await _tributeService.GetSubscriptions();
            return Ok(subscriptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving subscriptions");
            return StatusCode(500, new { error = "Internal server error retrieving subscriptions" });
        }
    }

    /// <summary>
    /// Get all donations (Admin endpoint)
    /// </summary>
    [HttpGet]
    [TelegramAuthorize]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> GetDonations()
    {
        try
        {
            var donations = await _tributeService.GetDonations();
            return Ok(donations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving donations");
            return StatusCode(500, new { error = "Internal server error retrieving donations" });
        }
    }
}

