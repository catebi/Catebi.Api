using Catebi.Api.Domain.Features.AdoptionBot.ViewModels;

namespace Catebi.Api.Domain.Features.AdoptionBot;

public interface IAdoptionBotEventService
{
    /// <summary>
    /// Get all events
    /// </summary>
    Task<IEnumerable<EventDto>> GetAllEvents();

    /// <summary>
    /// Get events by status
    /// </summary>
    Task<IEnumerable<EventDto>> GetEventsByStatus(string status);

    /// <summary>
    /// Get an event by record ID
    /// </summary>
    Task<EventDto?> GetEventById(string recordId);

    /// <summary>
    /// Add a new event
    /// </summary>
    Task<EventDto> AddEvent(EventDto event_);

    /// <summary>
    /// Update an event
    /// </summary>
    Task<EventDto> UpdateEvent(EventDto event_);

    /// <summary>
    /// Open registration for an event.
    /// </summary>
    Task<bool> OpenEventRegistration(string atEventId);

    /// <summary>
    /// Open registration for an event and notify all confirmed users.
    /// </summary>
    Task<bool> OpenEventRegistrationWithNotification(string atEventId);

    /// <summary>
    /// Close registration for an event.
    /// </summary>
    Task<bool> CloseEventRegistration(string atEventId);

    /// <summary>
    /// Get all cats registered for a specific event.
    /// </summary>
    Task<IEnumerable<CatDto>> GetEventCats(string eventRecordId);
} 