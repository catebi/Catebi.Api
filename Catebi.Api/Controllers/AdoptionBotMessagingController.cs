using Catebi.Api.Domain.Features.AdoptionBot;
using Catebi.Api.Domain.Features.AdoptionBot.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Catebi.Api.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class AdoptionBotMessagingController(IAdoptionBotAdminService AdminService) : ControllerBase
{
    /// <summary>
    /// Send a formatted message to all confirmed users
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> BroadcastMessage([FromBody] BroadcastMessageRequest request)
    {
        var result = await AdminService.BroadcastMessage(request.Content, request.AdminRecordId);
        return Ok(new ApiResponse
        {
            Success = true,
            Message = "📢 Message broadcasted to all confirmed users."
        });
    }

    /// <summary>
    /// Get all broadcast messages
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetBroadcastMessages()
    {
        var messages = await AdminService.GetBroadcastMessages();
        return Ok(new ApiResponse<IEnumerable<MessageDto>>
        {
            Success = true,
            Message = "Broadcast messages retrieved successfully.",
            Data = messages
        });
    }
}
