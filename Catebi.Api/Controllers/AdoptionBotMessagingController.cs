using Catebi.Api.Domain.Features.AdoptionBot;
using Catebi.Api.Domain.Features.AdoptionBot.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Catebi.Api.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class AdoptionBotMessagingController(IAdoptionBotAdminService AdminService,
                                          ILogger<AdoptionBotMessagingController> Logger) : ControllerBase
{
    /// <summary>
    /// Send a formatted message to all confirmed users
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> BroadcastMessage([FromBody] BroadcastMessageRequest request)
    {
        try
        {
            var result = await AdminService.BroadcastMessage(request.Content, request.AdminRecordId);
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "📢 Message broadcasted to all confirmed users."
            });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "⚠️ Error broadcasting message");
            return Ok(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Get all broadcast messages
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetBroadcastMessages()
    {
        try
        {
            var messages = await AdminService.GetBroadcastMessages();
            return Ok(new ApiResponse<IEnumerable<MessageDto>>
            {
                Success = true,
                Message = "Broadcast messages retrieved successfully.",
                Data = messages
            });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "⚠️ Error getting broadcast messages");
            return Ok(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
    }
}

public class BroadcastMessageRequest
{
    public string Content { get; set; }
    public string AdminRecordId { get; set; }
}
