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
    public class AddCatRequest
    {
        public string OwnerRecordId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Sex { get; set; } = string.Empty;
        public string DateOfBirth { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool? IsVaccinatedComplex { get; set; }
        public bool? IsVaccinatedRabies { get; set; }
    }

    public class UpdateCatRequest
    {
        public string? RecordId { get; set; }
        public string OwnerRecordId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Sex { get; set; } = string.Empty;
        public string DateOfBirth { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool? IsVaccinatedComplex { get; set; }
        public bool? IsVaccinatedRabies { get; set; }
        public string? OwnerNotes { get; set; }
    }

    /// <summary>
    /// Add a new cat record
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> AddCat([FromForm] AddCatRequest request, IFormFile? mainPhoto = null)
    {
        try
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
        catch (Exception ex) when (ex.Message.Contains("must be confirmed by an admin"))
        {
            Logger.LogWarning(ex, "⚠️ User not confirmed - cat creation blocked");
            return BadRequest(new {
                Error = "UserNotConfirmed",
                Message = ex.Message,
                Success = false
            });
        }
        catch (ArgumentException ex)
        {
            Logger.LogError(ex, "❌ File validation error while adding cat");
            return BadRequest(new { 
                Error = "FileValidationError",
                Message = ex.Message,
                Details = "This is likely a file format or size issue. Check the file type and size."
            });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "⚠️ Unexpected error adding cat");
            return StatusCode(500, new { 
                Error = "UnexpectedError",
                Message = ex.Message,
                StackTrace = ex.StackTrace
            });
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
    public async Task<IActionResult> UpdateCat([FromForm] UpdateCatRequest request, IFormFile? mainPhoto = null)
    {
        try
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
        catch (ArgumentException ex)
        {
            Logger.LogError(ex, "❌ File validation error while updating cat");
            return BadRequest(new { 
                Error = "FileValidationError",
                Message = ex.Message,
                Details = "This is likely a file format or size issue. Check the file type and size."
            });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "⚠️ Unexpected error updating cat");
            return StatusCode(500, new { 
                Error = "UnexpectedError",
                Message = ex.Message,
                StackTrace = ex.StackTrace
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddCatPhoto(string catRecordId, IFormFile file)
    {
        try
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
        catch (ArgumentException ex)
        {
            Logger.LogError(ex, "❌ File validation error in AddCatPhoto");
            return BadRequest(new { 
                Error = "FileValidationError",
                Message = ex.Message,
                Details = "This is likely a file format or size issue. Check the file type and size."
            });
        }
        catch(Exception ex)
        {
            Logger.LogError(ex, "⚠️ Unexpected error in AddCatPhoto");
            return StatusCode(500, new { 
                Error = "UnexpectedError",
                Message = ex.Message,
                StackTrace = ex.StackTrace
            });
        }
    }

    /// <summary>
    /// Register a cat to an event
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> RegisterCatToEvent([FromQuery] string catRecordId, [FromQuery] string eventRecordId)
    {
        try
        {
            var result = await CatService.RegisterCatToEvent(catRecordId, eventRecordId);
            if (result)
            {
                return Ok(new { Message = $"🎉 Cat successfully registered to event. Cat ID: {catRecordId}, Event ID: {eventRecordId}" });
            }
            else
            {
                return StatusCode(500, new { Message = $"Failed to register cat to event. Cat ID: {catRecordId}, Event ID: {eventRecordId}" });
            }
        }
        catch (ArgumentException ex)
        {
            Logger.LogError(ex, "⚠️ Error in RegisterCatToEvent - Validation error");
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "⚠️ Error in RegisterCatToEvent");
            return StatusCode(500, new { Message = $"An error occurred while registering cat to event: {ex.Message}" });
        }
    }
}
