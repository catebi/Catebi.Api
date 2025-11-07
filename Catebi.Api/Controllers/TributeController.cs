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
    /// Unified webhook endpoint for all Tribute events
    /// </summary>
    [HttpPost("Webhook")]
    public async Task<IActionResult> Webhook([FromBody] JsonElement rawRequest)
    {
        try
        {
            // Handle test event
            if (rawRequest.TryGetProperty("test_event", out var testEventValue))
            {
                _logger.LogInformation($"Received test event: {testEventValue.GetString()}");
                return Ok(new { success = true, message = "Test event received successfully" });
            }

            // Deserialize as normal webhook request
            var request = JsonSerializer.Deserialize<TributeWebhookRequest>(
                rawRequest.GetRawText(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (request == null)
            {
                _logger.LogError("Failed to deserialize webhook request");
                return BadRequest(new { error = "Invalid request" });
            }

            _logger.LogInformation($"Processing webhook: {request.Name} (CreatedAt: {request.CreatedAt}, SentAt: {request.SentAt})");

            // Route based on webhook event name
            return request.Name switch
            {
                "new_subscription" => await HandleNewSubscription(request),
                "cancelled_subscription" => await HandleCancelledSubscription(request),
                "new_donation" => await HandleNewDonation(request),
                "recurrent_donation" => await HandleRecurrentDonation(request),
                "cancelled_donation" => await HandleCancelledDonation(request),
                _ => BadRequest(new { error = $"Unknown webhook type: {request.Name}" })
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing Tribute webhook");
            return StatusCode(500, new { error = "Internal server error processing webhook" });
        }
    }

    private async Task<IActionResult> HandleNewSubscription(TributeWebhookRequest request)
    {
        try
        {
            var payloadJson = request.Payload.GetRawText();
            _logger.LogDebug($"Payload JSON: {payloadJson}");

            var payload = JsonSerializer.Deserialize<NewSubscriptionPayload>(
                payloadJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (payload == null)
            {
                _logger.LogError("Failed to deserialize subscription payload");
                return BadRequest(new { error = "Invalid payload format" });
            }

            _logger.LogInformation($"Parsed subscription - Name: '{payload.SubscriptionName}', Amount: {payload.Amount / 100.0:F2} {payload.Currency}, Period: {payload.Period}, TelegramUserId: {payload.TelegramUserId}");

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
            throw;
        }
    }

    private async Task<IActionResult> HandleCancelledSubscription(TributeWebhookRequest request)
    {
        try
        {
            var payloadJson = request.Payload.GetRawText();
            _logger.LogDebug($"Payload JSON: {payloadJson}");

            var payload = JsonSerializer.Deserialize<CancelledSubscriptionPayload>(
                payloadJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (payload == null)
            {
                _logger.LogError("Failed to deserialize cancelled subscription payload");
                return BadRequest(new { error = "Invalid payload format" });
            }

            _logger.LogInformation($"Parsed cancelled subscription - Name: '{payload.SubscriptionName}', Amount: {payload.Amount / 100.0:F2} {payload.Currency}, Period: {payload.Period}, TelegramUserId: {payload.TelegramUserId}, CancelReason: {payload.CancelReason}");

            var subscriptionDto = await _tributeService.ProcessCancelledSubscription(
                request.Name,
                payload,
                request.CreatedAt,
                request.SentAt);

            _logger.LogInformation($"Successfully processed cancelled subscription - RecordId: {subscriptionDto.RecordId}, SubscriptionId: {payload.SubscriptionId}, User: {payload.TelegramUserId}");

            return Ok(new
            {
                success = true,
                message = "Cancelled subscription processed successfully",
                recordId = subscriptionDto.RecordId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing cancelled subscription webhook");
            throw;
        }
    }

    private async Task<IActionResult> HandleNewDonation(TributeWebhookRequest request)
    {
        try
        {
            var payloadJson = request.Payload.GetRawText();
            _logger.LogDebug($"Payload JSON: {payloadJson}");

            var payload = JsonSerializer.Deserialize<NewDonationPayload>(
                payloadJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (payload == null)
            {
                _logger.LogError("Failed to deserialize new donation payload");
                return BadRequest(new { error = "Invalid payload format" });
            }

            _logger.LogInformation($"Parsed new donation - Name: '{payload.DonationName}', Amount: {payload.Amount / 100.0:F2} {payload.Currency}, Period: {payload.Period}, TelegramUserId: {payload.TelegramUserId}, Anonymous: {payload.Anonymously}, Message: {payload.Message}");

            var donationDto = await _tributeService.ProcessNewDonation(
                request.Name,
                payload,
                request.CreatedAt,
                request.SentAt);

            _logger.LogInformation($"Successfully processed new donation - RecordId: {donationDto.RecordId}, DonationRequestId: {payload.DonationRequestId}, User: {payload.TelegramUserId}");

            return Ok(new
            {
                success = true,
                message = "New donation processed successfully",
                recordId = donationDto.RecordId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing new donation webhook");
            throw;
        }
    }

    private async Task<IActionResult> HandleRecurrentDonation(TributeWebhookRequest request)
    {
        try
        {
            var payloadJson = request.Payload.GetRawText();
            _logger.LogDebug($"Payload JSON: {payloadJson}");

            var payload = JsonSerializer.Deserialize<RecurrentDonationPayload>(
                payloadJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (payload == null)
            {
                _logger.LogError("Failed to deserialize donation payload");
                return BadRequest(new { error = "Invalid payload format" });
            }

            _logger.LogInformation($"Parsed donation - Name: '{payload.DonationName}', Amount: {payload.Amount / 100.0:F2} {payload.Currency}, Period: {payload.Period}, TelegramUserId: {payload.TelegramUserId}, Anonymous: {payload.Anonymously}");

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
            throw;
        }
    }

    private async Task<IActionResult> HandleCancelledDonation(TributeWebhookRequest request)
    {
        try
        {
            var payloadJson = request.Payload.GetRawText();
            _logger.LogDebug($"Payload JSON: {payloadJson}");

            var payload = JsonSerializer.Deserialize<CancelledDonationPayload>(
                payloadJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (payload == null)
            {
                _logger.LogError("Failed to deserialize cancelled donation payload");
                return BadRequest(new { error = "Invalid payload format" });
            }

            _logger.LogInformation($"Parsed cancelled donation - Name: '{payload.DonationName}', Amount: {payload.Amount / 100.0:F2} {payload.Currency}, Period: {payload.Period}, TelegramUserId: {payload.TelegramUserId}, Anonymous: {payload.Anonymously}");

            var donationDto = await _tributeService.ProcessCancelledDonation(
                request.Name,
                payload,
                request.CreatedAt,
                request.SentAt);

            _logger.LogInformation($"Successfully processed cancelled donation - RecordId: {donationDto.RecordId}, DonationRequestId: {payload.DonationRequestId}, User: {payload.TelegramUserId}");

            return Ok(new
            {
                success = true,
                message = "Cancelled donation processed successfully",
                recordId = donationDto.RecordId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing cancelled donation webhook");
            throw;
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

