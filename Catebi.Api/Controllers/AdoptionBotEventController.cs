using Catebi.Api.Domain.Features.AdoptionBot;
using Catebi.Api.Domain.Features.AdoptionBot.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Catebi.Api.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class AdoptionBotEventController(IAdoptionBotEventService EventService) : ControllerBase
{
    /// <summary>
    /// Get all events
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllEvents()
    {
        var events = await EventService.GetAllEvents();
        return Ok(events);
    }

    /// <summary>
    /// Get events by status
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetEventsByStatus([FromQuery] string status)
    {
        var events = await EventService.GetEventsByStatus(status);
        return Ok(events);
    }

    /// <summary>
    /// Get an event by record ID
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetEventById([FromQuery] string recordId)
    {
        var event_ = await EventService.GetEventById(recordId);
        if (event_ == null)
        {
            return NotFound(new { Message = $"Event with ID {recordId} not found." });
        }
        return Ok(event_);
    }

    /// <summary>
    /// Add a new event
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> AddEvent([FromBody] EventDto event_)
    {
        var result = await EventService.AddEvent(event_);
        return Ok(result);
    }

    /// <summary>
    /// Update an event
    /// </summary>
    [HttpPut]
    public async Task<IActionResult> UpdateEvent([FromBody] EventDto event_)
    {
        if (string.IsNullOrEmpty(event_.RecordId))
        {
            return BadRequest(new { Message = "Record ID is required for updating an event." });
        }

        var result = await EventService.UpdateEvent(event_);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> OpenEventRegistration([FromQuery] string id)
    {
        var result = await EventService.OpenEventRegistration(id);
        if (result)
        {
            return Ok(new { Message = $"🎉 event registration ({id}) opened." });
        }

        return StatusCode(500, new { Message = $"Failed to open event registration {id}." });
    }

    [HttpGet]
    public async Task<IActionResult> CloseEventRegistration([FromQuery] string id)
    {
        var result = await EventService.CloseEventRegistration(id);
        if (result)
        {
            return Ok(new { Message = $"🎉 event registration ({id}) closed." });
        }

        return StatusCode(500, new { Message = $"Failed to close event registration {id}." });
    }

    /// <summary>
    /// Get all cats registered for a specific event
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetEventCats([FromQuery] string eventRecordId)
    {
        var cats = await EventService.GetEventCats(eventRecordId);
        return Ok(cats);
    }
} 