using Catebi.Api.Domain.Features.AdoptionBot;
using Catebi.Api.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Catebi.Api.Domain.Features.AdoptionBot.ViewModels;

namespace Catebi.Api.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class AdoptionBotCatPaymentController(IAdoptionBotCatService CatService,
                                      IFileService FileService,
                                      ILogger<AdoptionBotCatPaymentController> Logger) : ControllerBase
{
    /// <summary>
    /// Get payments for a specific cat
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetCatPayments([FromQuery] string catRecordId)
    {
        try
        {
            var payments = await CatService.GetCatPayments(catRecordId);
            return Ok(payments);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "⚠️ Error getting cat payments");
            return StatusCode(500, new { ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddCatPayment(string catRecordId, IFormFile file)
    {
        try
        {
            Logger.LogInformation($"Received file for cat payment confirmation: {file.FileName}");
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            var extension = Path.GetExtension(file.FileName);
            var fileName = Path.GetFileName(file.FileName) ?? $"confirmation_{catRecordId}.{extension}";
            var fileSize = file.Length;
            var fileType = file.ContentType;

            var fileRequest = new FileStorageDto
            {
                FileName = fileName,
                Size = fileSize,
                ContentType = fileType,
                Data = memoryStream.ToArray()
            };

            var uploadedFile = await FileService.SaveFileAsync(fileRequest);

            Logger.LogInformation($"File uploaded successfully: {uploadedFile.FileStorageId}");

            fileRequest.FileStorageId = uploadedFile.FileStorageId;
            var fileUrl = FileService.GenerateFileUrl(fileRequest);
            fileUrl = "https://v5.airtableusercontent.com/v3/u/42/42/1751126400000/kCOthkH6Z9w-ZL9Uzsi8mA/y8unoGdVhYmN9lt1XifWPhb1tcJ-Im09E9ix-dw6zdQjAnFqGUbzcGHKtWkJt1j3_Y8LACXwK6tkM7QlPWMhMRG42K0DRdvVxSY6uNOTSzD4Qt0qEFQ-J1fgpLtcD3OMFGgdxK6zwyMfHTq-K0ogNQ/uiSfC0is0PfCVSnbM3pe8BIgzvYgvMkvjRD98tY3Xek";

            Logger.LogInformation($"Generated file URL: {fileUrl}");

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
        catch (ArgumentException ex)
        {
            Logger.LogError(ex, "⚠️ Error in AddCatPayment");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "⚠️ Error in AddCatPayment");
            return StatusCode(500, $"An error occurred while processing your request., exception: {ex.Message}");
        }
    }
}
