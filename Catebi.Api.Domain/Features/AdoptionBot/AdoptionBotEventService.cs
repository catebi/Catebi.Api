using AirtableApiClient;
using Telegram.Bot;
using Catebi.Api.Domain.Features.AdoptionBot.Enums;
using Catebi.Api.Domain.Features.AdoptionBot.Models;
using Catebi.Api.Domain.Features.AdoptionBot.Converters;

namespace Catebi.Api.Domain.Features.AdoptionBot;

public class AdoptionBotEventService(
    IAirtableRepository AirtableRepository,
    TelegramBotClient TelegramBotClient,
    ILogger<AdoptionBotEventService> Logger) : IAdoptionBotEventService
{
    private readonly string EventTableName = AirTables.Event.ToString();
    private readonly string StatusColumnName = "Status";

    public async Task<IEnumerable<EventDto>> GetAllEvents()
    {
        Logger.LogInformation("Getting all events");

        var response = await AirtableRepository.ListRecords<AtEvent>(
            EventTableName,
            filterByFormula: null
        );

        if (!response.Success)
        {
            Logger.LogError($"Error getting events: {response.AirtableApiError.ErrorMessage}");
            throw new Exception($"Error getting events: {response.AirtableApiError.ErrorMessage}");
        }

        var events = response.Records.Select(r => EventConverter.ToDto(r.Fields, r.Id)).ToList();
        Logger.LogInformation($"Found {events.Count} events");
        return events;
    }

    public async Task<IEnumerable<EventDto>> GetEventsByStatus(string status)
    {
        Logger.LogInformation($"Getting events with status: {status}");

        var response = await AirtableRepository.ListRecords<AtEvent>(
            EventTableName,
            filterByFormula: $"{{Status}} = '{status}'"
        );

        if (!response.Success)
        {
            Logger.LogError($"Error getting events by status: {response.AirtableApiError.ErrorMessage}");
            throw new Exception($"Error getting events by status: {response.AirtableApiError.ErrorMessage}");
        }

        var events = response.Records.Select(r => EventConverter.ToDto(r.Fields, r.Id)).ToList();
        Logger.LogInformation($"Found {events.Count} events with status {status}");
        return events;
    }

    public async Task<EventDto?> GetEventById(string recordId)
    {
        Logger.LogInformation($"Getting event by ID: {recordId}");

        var response = await AirtableRepository.RetrieveRecord<AtEvent>(EventTableName, recordId);

        if (!response.Success)
        {
            Logger.LogError($"Error getting event: {response.AirtableApiError.ErrorMessage}");
            throw new Exception($"Error getting event: {response.AirtableApiError.ErrorMessage}");
        }

        if (response.Record == null)
        {
            Logger.LogInformation($"No event found with ID: {recordId}");
            return null;
        }

        Logger.LogInformation($"Found event: {response.Record.Fields.Name}");
        return EventConverter.ToDto(response.Record.Fields, response.Record.Id);
    }

    public async Task<EventDto> AddEvent(EventDto eventDto)
    {
        var event_ = EventConverter.ToAtEvent(eventDto);
        Logger.LogInformation($"Adding new event: {event_.Name}");

        var fields = new Fields();
        fields.AddField("Name", event_.Name);
        fields.AddField("Description", event_.Description);
        fields.AddField("Status", event_.StatusValue);
        fields.AddField("When", event_.When);
        fields.AddField("Where", event_.Where);
        fields.AddField("Cats", event_.Cats);
        fields.AddField("PaidSlotCount", event_.PaidSlotCount);
        fields.AddField("FreeSlotCount", event_.FreeSlotCount);
        if (event_.Poster.Length > 0)
        {
            fields.AddField("Poster", event_.Poster.Select(p => new AirtableAttachment { Url = p.Url }).ToList());
        }

        var response = await AirtableRepository.CreateRecord(EventTableName, fields);

        if (!response.Success)
        {
            Logger.LogError($"Error adding event: {response.AirtableApiError.ErrorMessage}");
            throw new Exception($"Error adding event: {response.AirtableApiError.ErrorMessage}");
        }

        // Retrieve the created event to get all fields and the correct recordId
        var created = await AirtableRepository.RetrieveRecord<AtEvent>(EventTableName, response.Record.Id);
        if (!created.Success || created.Record == null)
        {
            throw new Exception($"Error retrieving created event: {response.AirtableApiError.ErrorMessage}");
        }
        return EventConverter.ToDto(created.Record.Fields, created.Record.Id);
    }

    public async Task<EventDto> UpdateEvent(EventDto eventDto)
    {
        if (string.IsNullOrEmpty(eventDto.RecordId))
        {
            throw new ArgumentException("Record ID is required for updating an event");
        }
        var event_ = EventConverter.ToAtEvent(eventDto);
        Logger.LogInformation($"Updating event: {event_.Name}");

        var fields = new Fields();
        fields.AddField("Name", event_.Name);
        fields.AddField("Description", event_.Description);
        fields.AddField("Status", event_.StatusValue);
        fields.AddField("When", event_.When);
        fields.AddField("Where", event_.Where);
        fields.AddField("Cats", event_.Cats);
        fields.AddField("PaidSlotCount", event_.PaidSlotCount);
        fields.AddField("FreeSlotCount", event_.FreeSlotCount);
        if (event_.Poster.Length > 0)
        {
            fields.AddField("Poster", event_.Poster.Select(p => new AirtableAttachment { Url = p.Url }).ToList());
        }

        var response = await AirtableRepository.UpdateRecord(EventTableName, fields, event_.RecordId);

        if (!response.Success)
        {
            Logger.LogError($"Error updating event: {response.AirtableApiError.ErrorMessage}");
            throw new Exception($"Error updating event: {response.AirtableApiError.ErrorMessage}");
        }

        // Retrieve the updated event to get all fields and the correct recordId
        var updated = await AirtableRepository.RetrieveRecord<AtEvent>(EventTableName, event_.RecordId);
        if (!updated.Success || updated.Record == null)
        {
            throw new Exception($"Error retrieving updated event: {response.AirtableApiError.ErrorMessage}");
        }
        return EventConverter.ToDto(updated.Record.Fields, updated.Record.Id);
    }

    public async Task<bool> OpenEventRegistration(string atEventId)
    {
        var event_ = await AirtableRepository.RetrieveRecord<AtEvent>(EventTableName, atEventId);

        if (!event_.Success || event_.Record == null)
        {
            throw new Exception($"Event with ID {atEventId} not found.");
        }

        var eventModel = event_.Record.Fields;

        if (eventModel.Status != EventStatuses.Closed)
        {
            throw new Exception($"Event {eventModel.Name} must be in Closed status.");
        }

        var updatedFields = new Fields();
        updatedFields.AddField(StatusColumnName, EventStatuses.Open.ToString());
        var updateResponse = await AirtableRepository.UpdateRecord(EventTableName, updatedFields, atEventId);

        if (!updateResponse.Success)
        {
            throw new Exception($"Error updating status for event {eventModel.Name}: {updateResponse.AirtableApiError.ErrorMessage}");
        }

        return true;
    }

    public async Task<bool> CloseEventRegistration(string atEventId)
    {
        var event_ = await AirtableRepository.RetrieveRecord<AtEvent>(EventTableName, atEventId);

        if (!event_.Success || event_.Record == null)
        {
            throw new Exception($"Event with ID {atEventId} not found.");
        }

        var eventModel = event_.Record.Fields;

        if (eventModel.Status != EventStatuses.Open)
        {
            throw new Exception($"Event {eventModel.Name} must be in Open status.");
        }

        var updatedFields = new Fields();
        updatedFields.AddField(StatusColumnName, EventStatuses.Closed.ToString());
        var updateResponse = await AirtableRepository.UpdateRecord(EventTableName, updatedFields, atEventId);

        if (!updateResponse.Success)
        {
            throw new Exception($"Error updating status for event {eventModel.Name}: {updateResponse.AirtableApiError.ErrorMessage}");
        }

        return true;
    }
} 