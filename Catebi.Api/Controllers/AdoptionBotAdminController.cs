using Catebi.Api.Domain.Features.AdoptionBot;
using Catebi.Api.Domain.Features.AdoptionBot.ViewModels;
using Catebi.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Catebi.Api.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace Catebi.Api.Controllers;

[Route("[controller]/[action]")]
[ApiController]
[TelegramAuthorize]
[Authorize(Policy = "Admin")]
public class AdoptionBotAdminController(IAdoptionBotAdminService AdminService) : ControllerBase
{
    /// <summary>
    /// User account confirmation with additional details
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> ConfirmUser([FromBody] ConfirmUserRequest request)
    {
        // Get admin info from claims for audit trail
        var adminTelegramId = User.FindFirst("TelegramId")?.Value;
        var adminName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

        var result = await AdminService.ConfirmUser(request.RecordId, request.IsVolunteer, request.Notes);

        if (result)
        {
            return Ok(new ApiResponse
            {
                Success = true,
                Message = $"🎉 User {request.RecordId} confirmed and notified by admin {adminName}."
            });
        }

        return BadRequest(new ApiResponse
        {
            Success = false,
            Message = "Failed to confirm user"
        });
    }

    /// <summary>
    /// Get all users that need confirmation
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetUsersToConfirm()
    {
        var users = await AdminService.GetUsersToConfirm();
        return Ok(users);
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmCatPayment([FromQuery] string paymentRecordId)
    {
        // Get admin info from claims for audit trail
        var adminName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

        var result = await AdminService.ConfirmCatPayment(paymentRecordId);

        if (result)
        {
            return Ok(new ApiResponse
            {
                Success = true,
                Message = $"🎉 Cat payment ({paymentRecordId}) confirmed and notified by admin {adminName}."
            });
        }

        return BadRequest(new ApiResponse
        {
            Success = false,
            Message = "Failed to confirm payment"
        });
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CatPaymentDto>>> GetPaymentsToConfirm()
    {
        var payments = await AdminService.GetPaymentsToConfirm();
        return Ok(payments);
    }

    [HttpGet]
    public async Task<IActionResult> GetBroadcastMessages()
    {
        var messages = await AdminService.GetBroadcastMessages();
        return Ok(messages);
    }
}
