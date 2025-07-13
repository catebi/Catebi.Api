using Catebi.Api.Domain.Features.AdoptionBot;
using Catebi.Api.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Catebi.Api.Domain.Features.AdoptionBot.ViewModels;

namespace Catebi.Api.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class AdoptionBotCatPaymentController(IAdoptionBotCatService CatService, IFileService FileService) : ControllerBase
{
    /// <summary>
    /// Get payments for a specific cat
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetCatPayments([FromQuery] string catRecordId)
    {
        var payments = await CatService.GetCatPayments(catRecordId);
        return Ok(payments);
    }

    [HttpPost]
    public async Task<IActionResult> AddCatPayment(string catRecordId, IFormFile file)
    {
        var fileUrl = await FileService.ProcessFileUploadAsync(file, $"confirmation_{catRecordId}");

        var result = await CatService.AddCatPayment(catRecordId, fileUrl);
        if (result != null)
        {
            return Ok(result);
        }
        else
        {
            return StatusCode(500, new { Message = $"Failed to create cat payment {catRecordId}." });
        }
    }
}
