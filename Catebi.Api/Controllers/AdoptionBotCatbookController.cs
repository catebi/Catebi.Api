using Catebi.Api.Domain.Features.AdoptionBot;
using Catebi.Api.Domain.Features.AdoptionBot.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Catebi.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Catebi.Api.Models;

namespace Catebi.Api.Controllers;

[Route("[controller]/[action]")]
[ApiController]
[TelegramAuthorize]
public class AdoptionBotCatbookController(IAdoptionBotCatbookService CatbookService) : ControllerBase
{
    /// <summary>
    /// Get catbook information by cat record ID
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "RegisteredUser")]
    public async Task<IActionResult> GetCatbookInfo([FromQuery] string catRecordId)
    {
        if (string.IsNullOrEmpty(catRecordId))
        {
            return BadRequest(new { Message = "Cat record ID is required." });
        }

        var catbookInfo = await CatbookService.GetCatbookInfo(catRecordId);
        if (catbookInfo == null)
        {
            return NotFound(new { Message = $"No catbook information found for cat ID {catRecordId}." });
        }

        return Ok(catbookInfo);
    }

    /// <summary>
    /// Save new catbook information with ToConfirm status
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "RegisteredUser")]
    public async Task<IActionResult> SaveCatbookInfo([FromBody] CatbookInfoDto catbookInfo)
    {
        if (catbookInfo == null)
        {
            return BadRequest(new { Message = "Catbook information is required." });
        }

        if (string.IsNullOrEmpty(catbookInfo.CatRecordId))
        {
            return BadRequest(new { Message = "Cat record ID is required." });
        }

        var result = await CatbookService.SaveCatbookInfo(catbookInfo);
        return Ok(new ApiResponse<CatbookInfoDto>
        {
            Success = true,
            Message = "🎉 Catbook information saved successfully and submitted for confirmation.",
            Data = result
        });
    }

    /// <summary>
    /// Update existing catbook information (except status)
    /// </summary>
    [HttpPut]
    [Authorize(Policy = "RegisteredUser")]
    public async Task<IActionResult> UpdateCatbookInfo([FromBody] CatbookInfoDto catbookInfo)
    {
        if (catbookInfo == null)
        {
            return BadRequest(new { Message = "Catbook information is required." });
        }

        if (string.IsNullOrEmpty(catbookInfo.RecordId))
        {
            return BadRequest(new { Message = "Record ID is required for updating catbook information." });
        }

        var result = await CatbookService.UpdateCatbookInfo(catbookInfo);
        return Ok(new ApiResponse<CatbookInfoDto>
        {
            Success = true,
            Message = "🎉 Catbook information updated successfully.",
            Data = result
        });
    }

    /// <summary>
    /// Confirm catbook information and send new post to the catbook telegram channel (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> ConfirmCatbookInfo([FromQuery] string catRecordId)
    {
        if (string.IsNullOrEmpty(catRecordId))
        {
            return BadRequest(new { Message = "Cat record ID is required." });
        }

        // Get admin info from claims for audit trail
        var adminName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

        var result = await CatbookService.ConfirmCatbookInfo(catRecordId);

        if (result)
        {
            return Ok(new ApiResponse
            {
                Success = true,
                Message = $"🎉 Catbook information for cat {catRecordId} confirmed and posted to Telegram channel by admin {adminName}."
            });
        }

        return BadRequest(new ApiResponse
        {
            Success = false,
            Message = "Failed to confirm catbook information"
        });
    }

    /// <summary>
    /// Archive catbook information (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> ArchiveCatbookInfo([FromQuery] string catRecordId)
    {
        if (string.IsNullOrEmpty(catRecordId))
        {
            return BadRequest(new { Message = "Cat record ID is required." });
        }

        // Get admin info from claims for audit trail
        var adminName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

        var result = await CatbookService.ArchiveCatbookInfo(catRecordId);

        if (result)
        {
            return Ok(new ApiResponse
            {
                Success = true,
                Message = $"🎉 Catbook information for cat {catRecordId} archived by admin {adminName}."
            });
        }

        return BadRequest(new ApiResponse
        {
            Success = false,
            Message = "Failed to archive catbook information"
        });
    }

    /// <summary>
    /// Get all catbook information records with ToConfirm status for admin
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> GetCatbookInfosToConfirm()
    {
        var catbookInfos = await CatbookService.GetCatbookInfosToConfirm();
        return Ok(new ApiResponse<IEnumerable<CatbookInfoDto>>
        {
            Success = true,
            Message = "Catbook information records to confirm retrieved successfully.",
            Data = catbookInfos
        });
    }
}
