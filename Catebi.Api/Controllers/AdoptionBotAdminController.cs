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
                return Ok(new ConfirmUserResponse 
                { 
                    Success = true, 
                    Message = $"🎉 user {request.RecordId} confirmed and notified." 
                });
            }

            return Ok(new ConfirmUserResponse 
            { 
                Success = false, 
                Message = $"Failed to confirm user {request.RecordId}." 
            });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "⚠️ Error confirming user");
            return Ok(new ConfirmUserResponse 
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
    public async Task<IActionResult> GetUsersToConfirm()
    {
        try
        {
            var users = await AdminService.GetUsersToConfirm();
            return Ok(users);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "⚠️ Error getting users to confirm");
            return StatusCode(500, new { ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmCatPayment([FromQuery] string id)
    {
        try
        {
            var result = await AdminService.ConfirmCatPayment(id);
            if (result)
            {
                return Ok(new { Message = $"🎉 cat payment ({id}) confirmed and notified." });
            }

            return StatusCode(500, new { Message = $"Failed to confirm cat payment {id}." });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "⚠️ Error confirming cat payment");
            return StatusCode(500, new { ex.Message });
        }
    }
}

public class ConfirmUserRequest
{
    public string RecordId { get; set; }
    public bool IsVolunteer { get; set; }
    public string? Notes { get; set; }
}

public class ConfirmUserResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
}
