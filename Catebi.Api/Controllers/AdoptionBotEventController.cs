using Catebi.Api.Domain.Features.AdoptionBot;
using Catebi.Api.Domain.Features.AdoptionBot.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Catebi.Api.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class AdoptionBotEventController(IAdoptionBotEventService EventService,
                                      ILogger<AdoptionBotEventController> Logger) : ControllerBase
{
    /// <summary>
    /// Get all events
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllEvents()
    {
        try
        {
            var events = await EventService.GetAllEvents();
            return Ok(events);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "⚠️ Error getting all events");
            return StatusCode(500, new { ex.Message });
        }
    }

    /// <summary>
    /// Get events by status
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetEventsByStatus([FromQuery] string status)
    {
        try
        {
            var events = await EventService.GetEventsByStatus(status);
            return Ok(events);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "⚠️ Error getting events by status");
            return StatusCode(500, new { ex.Message });
        }
    }

    /// <summary>
    /// Get an event by record ID
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetEventById([FromQuery] string recordId)
    {
        try
        {
            var event_ = await EventService.GetEventById(recordId);
            if (event_ == null)
            {
                return NotFound(new { Message = $"Event with ID {recordId} not found." });
            }
            return Ok(event_);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "⚠️ Error getting event by ID");
            return StatusCode(500, new { ex.Message });
        }
    }

    /// <summary>
    /// Add a new event
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> AddEvent([FromBody] EventDto event_)
    {
        try
        {
            var result = await EventService.AddEvent(event_);
            return Ok(result);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "⚠️ Error adding event");
            return StatusCode(500, new { ex.Message });
        }
    }

    /// <summary>
    /// Update an event
    /// </summary>
    [HttpPut]
    public async Task<IActionResult> UpdateEvent([FromBody] EventDto event_)
    {
        try
        {
            if (string.IsNullOrEmpty(event_.RecordId))
            {
                return BadRequest(new { Message = "Record ID is required for updating an event." });
            }

            var result = await EventService.UpdateEvent(event_);
            return Ok(result);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "⚠️ Error updating event");
            return StatusCode(500, new { ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> OpenEventRegistration([FromQuery] string id)
    {
        try
        {
            var result = await EventService.OpenEventRegistration(id);
            if (result)
            {
                return Ok(new { Message = $"🎉 event registration ({id}) opened." });
            }

            return StatusCode(500, new { Message = $"Failed to open event registration {id}." });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "⚠️ Error opening event registration");
            return StatusCode(500, new { ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> CloseEventRegistration([FromQuery] string id)
    {
        try
        {
            var result = await EventService.CloseEventRegistration(id);
            if (result)
            {
                return Ok(new { Message = $"🎉 event registration ({id}) closed." });
            }

            return StatusCode(500, new { Message = $"Failed to close event registration {id}." });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "⚠️ Error closing event registration");
            return StatusCode(500, new { ex.Message });
        }
    }
} 