using Catebi.Api.Domain.Features.AdoptionBot;
using Catebi.Api.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Catebi.Api.Domain.Features.AdoptionBot.ViewModels;
using Catebi.Api.Models;

namespace Catebi.Api.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public partial class AdoptionBotCatController(IAdoptionBotCatService CatService,
                                      IFileService FileService,
                                      ILogger<AdoptionBotCatController> Logger) : ControllerBase
{

    /// <summary>
    /// Add a new cat record
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> AddCat([FromForm] AddCatRequest request, IFormFile? mainPhoto = null)
    {
        string? mainPhotoUrl = null;

        // Handle main photo upload if provided
        if (mainPhoto != null)
        {
            Logger.LogInformation($"📄 Received main photo for new cat: {mainPhoto.FileName}");
            Logger.LogInformation($"📊 File details - Size: {mainPhoto.Length} bytes, ContentType: '{mainPhoto.ContentType}'");

            using var memoryStream = new MemoryStream();
            await mainPhoto.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            var extension = Path.GetExtension(mainPhoto.FileName);
            var fileName = Path.GetFileName(mainPhoto.FileName) ?? $"main_photo_{DateTime.UtcNow:yyyyMMddHHmmss}.{extension}";
            var fileSize = mainPhoto.Length;
            var fileType = mainPhoto.ContentType;

            Logger.LogInformation($"🔄 Processing file - Name: '{fileName}', Size: {fileSize}, Type: '{fileType}', Extension: '{extension}'");

            var fileRequest = new FileStorageDto
            {
                FileName = fileName,
                Size = fileSize,
                ContentType = fileType,
                Data = memoryStream.ToArray()
            };

            Logger.LogInformation($"💾 About to save file to storage...");
            var uploadedFile = await FileService.SaveFileAsync(fileRequest);
            Logger.LogInformation($"✅ Main photo uploaded successfully: {uploadedFile.FileStorageId}");

            fileRequest.FileStorageId = uploadedFile.FileStorageId;
            mainPhotoUrl = FileService.GenerateFileUrl(fileRequest);
            Logger.LogInformation($"🔗 Generated main photo URL: {mainPhotoUrl}");
        }

        // Create CatDto from request
        var cat = new CatDto
        {
            OwnerRecordId = request.OwnerRecordId,
            Name = request.Name,
            Sex = request.Sex,
            DateOfBirth = request.DateOfBirth,
            Status = request.Status,
            IsVaccinatedComplex = request.IsVaccinatedComplex,
            IsVaccinatedRabies = request.IsVaccinatedRabies
        };

        // Set the main photo URL in the cat DTO if uploaded
        if (!string.IsNullOrEmpty(mainPhotoUrl))
        {
            cat.MainPhoto = new AttachmentDto
            {
                Url = mainPhotoUrl,
                Filename = mainPhoto?.FileName
            };
        }

        var result = await CatService.AddCat(cat);
        return Ok(result);
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
    /// Update a cat record
    /// </summary>
    [HttpPut]
    public async Task<IActionResult> UpdateCat([FromForm] UpdateCatRequest request, IFormFile? mainPhoto = null)
    {
        string? mainPhotoUrl = null;

        // Handle main photo upload if provided
        if (mainPhoto != null)
        {
            Logger.LogInformation($"📄 Received main photo for cat update: {mainPhoto.FileName}");
            Logger.LogInformation($"📊 File details - Size: {mainPhoto.Length} bytes, ContentType: '{mainPhoto.ContentType}'");

            using var memoryStream = new MemoryStream();
            await mainPhoto.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            var extension = Path.GetExtension(mainPhoto.FileName);
            var fileName = Path.GetFileName(mainPhoto.FileName) ?? $"main_photo_{DateTime.UtcNow:yyyyMMddHHmmss}.{extension}";
            var fileSize = mainPhoto.Length;
            var fileType = mainPhoto.ContentType;

            Logger.LogInformation($"🔄 Processing file - Name: '{fileName}', Size: {fileSize}, Type: '{fileType}', Extension: '{extension}'");

            var fileRequest = new FileStorageDto
            {
                FileName = fileName,
                Size = fileSize,
                ContentType = fileType,
                Data = memoryStream.ToArray()
            };

            Logger.LogInformation($"💾 About to save file to storage...");
            var uploadedFile = await FileService.SaveFileAsync(fileRequest);
            Logger.LogInformation($"✅ Main photo uploaded successfully: {uploadedFile.FileStorageId}");

            fileRequest.FileStorageId = uploadedFile.FileStorageId;
            mainPhotoUrl = FileService.GenerateFileUrl(fileRequest);
            Logger.LogInformation($"🔗 Generated main photo URL: {mainPhotoUrl}");
        }

        // Create CatDto from request
        var cat = new CatDto
        {
            RecordId = request.RecordId,
            OwnerRecordId = request.OwnerRecordId,
            Name = request.Name,
            Sex = request.Sex,
            DateOfBirth = request.DateOfBirth,
            Status = request.Status,
            IsVaccinatedComplex = request.IsVaccinatedComplex,
            IsVaccinatedRabies = request.IsVaccinatedRabies,
            OwnerNotes = request.OwnerNotes
        };

        // Set the main photo URL in the cat DTO if uploaded
        if (!string.IsNullOrEmpty(mainPhotoUrl))
        {
            cat.MainPhoto = new AttachmentDto
            {
                Url = mainPhotoUrl,
                Filename = mainPhoto?.FileName
            };
        }

        var result = await CatService.UpdateCat(cat);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddCatPhoto(string catRecordId, IFormFile file)
    {
        Logger.LogInformation($"📄 Received file for cat record: {file.FileName}");
        Logger.LogInformation($"📊 File details - Size: {file.Length} bytes, ContentType: '{file.ContentType}'");

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        memoryStream.Position = 0;
        var extension = Path.GetExtension(file.FileName);
        var fileName = Path.GetFileName(file.FileName) ?? $"cat_{catRecordId}.{extension}";
        var fileSize = file.Length;
        var fileType = file.ContentType;

        Logger.LogInformation($"🔄 Processing file - Name: '{fileName}', Size: {fileSize}, Type: '{fileType}', Extension: '{extension}'");

        var fileRequest = new FileStorageDto
        {
            FileName = fileName,
            Size = fileSize,
            ContentType = fileType,
            Data = memoryStream.ToArray()
        };

        Logger.LogInformation($"💾 About to save file to storage...");
        var uploadedFile = await FileService.SaveFileAsync(fileRequest);

        Logger.LogInformation($"✅ File uploaded successfully: {uploadedFile.FileStorageId}");

        fileRequest.FileStorageId = uploadedFile.FileStorageId;
        var fileUrl = FileService.GenerateFileUrl(fileRequest);

        Logger.LogInformation($"🔗 Generated file URL: {fileUrl}");

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

    /// <summary>
    /// Register a cat to an event with user validation (owner or admin only)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> RegisterCatToEvent([FromBody] CatToEventRequest request)
    {
        var result = await CatService.RegisterCatToEvent(request);
        if (result)
        {
            return Ok(new { Message = $"🎉 Cat successfully registered to event. Cat ID: {request.CatRecordId}, Event ID: {request.EventRecordId}" });
        }
        else
        {
            return StatusCode(500, new { Message = $"Failed to register cat to event. Cat ID: {request.CatRecordId}, Event ID: {request.EventRecordId}" });
        }
    }

    /// <summary>
    /// Remove a cat from an event with user validation (owner or admin only)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> RemoveCatFromEvent([FromBody] CatToEventRequest request)
    {
        var result = await CatService.RemoveCatFromEvent(request);
        if (result)
        {
            return Ok(new { Message = $"🎉 Cat successfully removed from event. Cat ID: {request.CatRecordId}, Event ID: {request.EventRecordId}" });
        }
        else
        {
            return StatusCode(500, new { Message = $"Failed to remove cat from event. Cat ID: {request.CatRecordId}, Event ID: {request.EventRecordId}" });
        }
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



}
