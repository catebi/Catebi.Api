using Catebi.Api.Domain.Features.AdoptionBot;
using Catebi.Api.Domain.Features.AdoptionBot.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Catebi.Api.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class AdoptionBotAdminCatController(IAdoptionBotCatService CatService) : ControllerBase
{
    /// <summary>
    /// Get cats with filtering and pagination for admin management
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> GetCats([FromBody] GetCatsForAdminRequest request)
    {
        var cats = await CatService.GetCatsForAdmin(request);
        return Ok(cats);
    }

    /// <summary>
    /// Get a cat by record ID
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetCatById([FromQuery] string recordId)
    {
        var cat = await CatService.GetCatById(recordId);
        if (cat == null)
        {
            return NotFound(new { Message = $"Cat with ID {recordId} not found." });
        }
        return Ok(cat);
    }

    /// <summary>
    /// Mark a cat as adopted with user validation (owner or admin only)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> MarkCatAsAdopted([FromBody] MarkCatAsAdoptedRequest request)
    {
        var result = await CatService.MarkCatAsAdopted(request.CatRecordId, request.UserRecordId, request.AdoptionComment);
        return Ok(new ApiResponse
        {
            Success = result,
            Message = result
                ? $"🎉 Cat successfully marked as adopted. Cat ID: {request.CatRecordId}"
                : $"Failed to mark cat as adopted. Cat ID: {request.CatRecordId}"
        });
    }

    /// <summary>
    /// Register a cat to an event with user validation (owner or admin only)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> RegisterCatToEvent([FromBody] CatToEventRequest request)
    {
        var result = await CatService.RegisterCatToEvent(request);
        return Ok(new ApiResponse
        {
            Success = result,
            Message = result
                ? $"🎉 Cat successfully registered to event. Cat ID: {request.CatRecordId}, Event ID: {request.EventRecordId}"
                : $"Failed to register cat to event. Cat ID: {request.CatRecordId}, Event ID: {request.EventRecordId}"
        });
    }

    /// <summary>
    /// Remove a cat from an event with user validation (owner or admin only)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> RemoveCatFromEvent([FromBody] CatToEventRequest request)
    {
        var result = await CatService.RemoveCatFromEvent(request);
        return Ok(new ApiResponse
        {
            Success = result,
            Message = result
                ? $"🎉 Cat successfully removed from event. Cat ID: {request.CatRecordId}, Event ID: {request.EventRecordId}"
                : $"Failed to remove cat from event. Cat ID: {request.CatRecordId}, Event ID: {request.EventRecordId}"
        });
    }

}
