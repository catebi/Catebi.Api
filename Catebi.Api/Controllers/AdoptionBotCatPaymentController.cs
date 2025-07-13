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
        var payments = await CatService.GetCatPayments(catRecordId);
        return Ok(payments);
    }

    [HttpPost]
    public async Task<IActionResult> AddCatPayment(string catRecordId, IFormFile file)
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
}
