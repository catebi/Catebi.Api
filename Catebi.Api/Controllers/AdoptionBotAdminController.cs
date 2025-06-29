using Catebi.Api.Domain.Features.AdoptionBot;
using Catebi.Api.Domain.Features.AdoptionBot.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Catebi.Api.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class AdoptionBotAdminController(IAdoptionBotAdminService AdminService,
                                      ILogger<AdoptionBotAdminController> Logger) : ControllerBase
{
    /// <summary>
    /// User account confirmation with additional details
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> ConfirmUser([FromBody] ConfirmUserRequest request)
    {
        try
        {
            var result = await AdminService.ConfirmUser(request.RecordId, request.IsVolunteer, request.Notes);
            if (result)
            {
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = $"🎉 user {request.RecordId} confirmed and notified."
                });
            }

            return Ok(new ApiResponse
            {
                Success = false,
                Message = $"Failed to confirm user {request.RecordId}."
            });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "⚠️ Error confirming user");
            return Ok(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Get all users that need confirmation
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersToConfirm()
    {
        try
        {
            var users = await AdminService.GetUsersToConfirm();
            return Ok(users);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting users to confirm");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmCatPayment([FromQuery] string paymentRecordId)
    {
        try
        {
            var result = await AdminService.ConfirmCatPayment(paymentRecordId);
            return Ok(new ApiResponse
            {
                Success = result,
                Message = result
                    ? $"🎉 cat payment ({paymentRecordId}) confirmed and notified."
                    : $"Failed to confirm cat payment {paymentRecordId}."
            });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "⚠️ Error confirming cat payment");
            return Ok(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CatPaymentDto>>> GetPaymentsToConfirm()
    {
        try
        {
            var payments = await AdminService.GetPaymentsToConfirm();
            return Ok(payments);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting payments to confirm");
            return BadRequest(new { message = ex.Message });
        }
    }
}

public class ConfirmUserRequest
{
    public string RecordId { get; set; }
    public bool IsVolunteer { get; set; }
    public string? Notes { get; set; }
}
