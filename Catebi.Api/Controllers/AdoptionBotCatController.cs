using Catebi.Api.Domain.Features.AdoptionBot;
using Catebi.Api.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Catebi.Api.Domain.Features.AdoptionBot.ViewModels;
using Catebi.Api.Models;

namespace Catebi.Api.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class AdoptionBotCatController(IAdoptionBotCatService CatService, IFileService FileService) : ControllerBase
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
            mainPhotoUrl = await FileService.ProcessFileUploadAsync(mainPhoto, "main_photo");
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
            IsVaccinatedRabies = request.IsVaccinatedRabies,
            IsCatebiCat = request.IsCatebiCat
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
            mainPhotoUrl = await FileService.ProcessFileUploadAsync(mainPhoto, "main_photo");
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
            OwnerNotes = request.OwnerNotes,
            IsCatebiCat = request.IsCatebiCat
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
        var fileUrl = await FileService.ProcessFileUploadAsync(file, $"cat_{catRecordId}");

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
