using Catebi.Api.Domain.Features.AdoptionBot;
using Catebi.Api.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Catebi.Api.Domain.Features.AdoptionBot.ViewModels;

namespace Catebi.Api.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class AdoptionBotCatController(IAdoptionBotCatService CatService,
                                      IFileService FileService,
                                      ILogger<AdoptionBotCatController> Logger) : ControllerBase
{
    /// <summary>
    /// Add a new cat record
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> AddCat([FromBody] CatDto cat)
    {
        try
        {
            var result = await CatService.AddCat(cat);
            return Ok(result);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "⚠️ Error adding cat");
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
    /// Update a cat record
    /// </summary>
    [HttpPut]
    public async Task<IActionResult> UpdateCat([FromBody] CatDto cat)
    {
        try
        {
            var result = await CatService.UpdateCat(cat);
            return Ok(result);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "⚠️ Error updating cat");
            return StatusCode(500, new { ex.Message });
        }
    }

    /// <summary>
    /// Get payments for a specific cat
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetCatPayments([FromQuery] string catId)
    {
        try
        {
            var payments = await CatService.GetCatPayments(catId);
            return Ok(payments);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "⚠️ Error getting cat payments");
            return StatusCode(500, new { ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddCatPhoto(string catRecordId, IFormFile file)
    {
        try
        {
            Logger.LogInformation($"Received file for cat record: {file.FileName}");
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            var extension = Path.GetExtension(file.FileName);
            var fileName = Path.GetFileName(file.FileName) ?? $"cat_{catRecordId}.{extension}";
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

            var result = await CatService.AddCatPhoto(catRecordId, fileUrl);
            if (result)
            {
                return Ok(new { Message = $"🎉 cat photo ({catRecordId}) added and notified." });
            }
            else
            {
                return StatusCode(500, new { Message = $"Failed to add cat photo {catRecordId}." });
            }
        }
        catch (ArgumentException ex)
        {
            Logger.LogError(ex, "⚠️ Error in AddCatPhoto");
            return BadRequest(ex.Message);
        }
        catch(Exception ex)
        {
            Logger.LogError(ex, "⚠️ Error in AddCatPhoto");
            return StatusCode(500, $"An error occurred while processing your request., exception: {ex.Message}");
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

            Logger.LogInformation($"Generated file URL: {fileUrl}");

            var result = await CatService.AddCatPayment(catRecordId, fileUrl);
            if (result)
            {
                return Ok(new { Message = $"🎉 cat payment ({catRecordId}) confirmed and notified." });
            }
            else
            {
                return StatusCode(500, new { Message = $"Failed to confirm cat payment {catRecordId}." });
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
