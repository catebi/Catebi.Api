using Catebi.Api.Domain.Features.AdoptionBot;
using Catebi.Api.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Catebi.Api.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class AdoptionController(IAdoptionBotActionService AdoptionService,
                                IFileService FileService,
                                ILogger<AdoptionController> Logger) : ControllerBase
{
    /// <summary>
    /// User account confirmation
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Obsolete("this method is not necessary anymore")]
    [HttpGet]
    public async Task<IActionResult> ConfirmUser([FromQuery] string id)
    {
        try
        {
            var result = await AdoptionService.ConfirmUser(id);
            if (result)
            {
                return Ok(new { Message = $"🎉 user {id} confirmed and notified." });
            }

            return StatusCode(500, new { Message = $"Failed to confirm user {id}." });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "⚠️ Error confirming user");
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

            var result = await AdoptionService.AddCatPhoto(catRecordId, fileUrl);
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

            var result = await AdoptionService.AddCatPayment(catRecordId, fileUrl);
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

    [HttpGet]
    public async Task<IActionResult> ConfirmCatPayment([FromQuery] string id)
    {
        try
        {
            var result = await AdoptionService.ConfirmCatPayment(id);
            if (result)
            {
                return Ok(new { Message = $"🎉 cat payment ({id}) confirmed and notified." });
            }

            return StatusCode(500, new { Message = $"Failed to confirm cat payment {id}." });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "⚠️ Error confirming cat payment");
            return StatusCode(500, new { ex.Message });
        }
    }
}
