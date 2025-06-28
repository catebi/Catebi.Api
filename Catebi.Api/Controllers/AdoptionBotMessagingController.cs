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
            return Ok(new { Message = "📢 Message broadcasted to all confirmed users.", MessageRecord = result });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "⚠️ Error broadcasting message");
            return StatusCode(500, new { ex.Message });
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
            return Ok(messages);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "⚠️ Error getting broadcast messages");
            return StatusCode(500, new { ex.Message });
        }
    }
}

public class BroadcastMessageRequest
{
    public string Content { get; set; }
    public string AdminRecordId { get; set; }
} 