using Catebi.Api.Domain.Features.AdoptionBot;
using Catebi.Api.Domain.Features.AdoptionBot.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Catebi.Api.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class AdoptionBotAdminController(IAdoptionBotAdminService AdminService) : ControllerBase
{
    /// <summary>
    /// User account confirmation with additional details
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> ConfirmUser([FromBody] ConfirmUserRequest request)
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

    /// <summary>
    /// Get all users that need confirmation
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersToConfirm()
    {
        var users = await AdminService.GetUsersToConfirm();
        return Ok(users);
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmCatPayment([FromQuery] string paymentRecordId)
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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CatPaymentDto>>> GetPaymentsToConfirm()
    {
        var payments = await AdminService.GetPaymentsToConfirm();
        return Ok(payments);
    }
}