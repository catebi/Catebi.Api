using Catebi.Api.Domain.Features.AdoptionBot;
using Catebi.Api.Domain.Features.AdoptionBot.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Catebi.Api.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class AdoptionBotAdminCatController(IAdoptionBotCatService CatService,
                                         ILogger<AdoptionBotAdminCatController> Logger) : ControllerBase
{
    /// <summary>
    /// Get cats with filtering and pagination for admin management
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> GetCats([FromBody] GetCatsForAdminRequest request)
    {
        try
        {
            var cats = await CatService.GetCatsForAdmin(request);
            return Ok(cats);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "⚠️ Error getting cats for admin");
            return StatusCode(500, new { ex.Message });
        }
    }

    /// <summary>
    /// Get a cat by record ID
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetCatById([FromQuery] string recordId)
    {
        try
        {
            var cat = await CatService.GetCatById(recordId);
            if (cat == null)
            {
                return NotFound(new { Message = $"Cat with ID {recordId} not found." });
            }
            return Ok(cat);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "⚠️ Error getting cat by ID");
            return StatusCode(500, new { ex.Message });
        }
    }

    /// <summary>
    /// Mark a cat as adopted with user validation (owner or admin only)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> MarkCatAsAdopted([FromBody] MarkCatAsAdoptedRequest request)
    {
        try
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
        catch (ArgumentException ex)
        {
            Logger.LogError(ex, "⚠️ Error in MarkCatAsAdopted - Validation error");
            return Ok(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "⚠️ Error in MarkCatAsAdopted");
            return Ok(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Register a cat to an event with user validation (owner or admin only)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> RegisterCatToEvent([FromBody] CatToEventRequest request)
    {
        try
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
        catch (ArgumentException ex)
        {
            Logger.LogError(ex, "⚠️ Error in RegisterCatToEvent - Validation error");
            return Ok(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "⚠️ Error in RegisterCatToEvent");
            return Ok(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Remove a cat from an event with user validation (owner or admin only)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> RemoveCatFromEvent([FromBody] CatToEventRequest request)
    {
        try
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
        catch (ArgumentException ex)
        {
            Logger.LogError(ex, "⚠️ Error in RemoveCatFromEvent - Validation error");
            return Ok(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "⚠️ Error in RemoveCatFromEvent");
            return Ok(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

}
