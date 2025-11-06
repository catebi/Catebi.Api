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
            _logger.LogInformation($"Received new subscription webhook: {request.Name}");

            if (request.Name != "new_subscription")
            {
                _logger.LogWarning($"Invalid webhook name for subscription endpoint: {request.Name}");
                return BadRequest(new { error = "Invalid webhook name, expected 'new_subscription'" });
            }

            // Deserialize the payload
            var payload = JsonSerializer.Deserialize<NewSubscriptionPayload>(
                request.Payload.GetRawText(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (payload == null)
            {
                _logger.LogError("Failed to deserialize subscription payload");
                return BadRequest(new { error = "Invalid payload format" });
            }

            // Process the subscription
            var subscriptionDto = await _tributeService.ProcessNewSubscription(
                payload,
                request.CreatedAt,
                request.SentAt);

            _logger.LogInformation($"Successfully processed subscription with ID: {subscriptionDto.RecordId}");

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
            _logger.LogInformation($"Received recurrent donation webhook: {request.Name}");

            if (request.Name != "recurrent_donation")
            {
                _logger.LogWarning($"Invalid webhook name for donation endpoint: {request.Name}");
                return BadRequest(new { error = "Invalid webhook name, expected 'recurrent_donation'" });
            }

            // Deserialize the payload
            var payload = JsonSerializer.Deserialize<RecurrentDonationPayload>(
                request.Payload.GetRawText(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (payload == null)
            {
                _logger.LogError("Failed to deserialize donation payload");
                return BadRequest(new { error = "Invalid payload format" });
            }

            // Process the donation
            var donationDto = await _tributeService.ProcessRecurrentDonation(
                payload,
                request.CreatedAt,
                request.SentAt);

            _logger.LogInformation($"Successfully processed donation with ID: {donationDto.RecordId}");

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

