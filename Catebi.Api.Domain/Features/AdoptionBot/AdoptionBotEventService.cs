using AirtableApiClient;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Catebi.Api.Domain.Features.AdoptionBot.Enums;
using Catebi.Api.Domain.Features.AdoptionBot.Models;
using Catebi.Api.Domain.Features.AdoptionBot.Converters;
using Catebi.Api.Domain.Contracts.Services;

namespace Catebi.Api.Domain.Features.AdoptionBot;

public class AdoptionBotEventService(
    IAirtableRepository AirtableRepository,
    TelegramBotClient TelegramBotClient,
    ILocalizationService LocalizationService,
    ILogger<AdoptionBotEventService> Logger) : IAdoptionBotEventService
{
    private readonly string EventTableName = AirTables.Event.ToString();
    private readonly string CatTableName = AirTables.Cat.ToString();
    private readonly string UserTableName = AirTables.User.ToString();
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
        updatedFields.AddField(StatusColumnName, EventStatuses.BookingOpen.ToString());
        var updateResponse = await AirtableRepository.UpdateRecord(EventTableName, updatedFields, atEventId);

        if (!updateResponse.Success)
        {
            throw new Exception($"Error updating status for event {eventModel.Name}: {updateResponse.AirtableApiError.ErrorMessage}");
        }

        // Get all confirmed users and notify them
        var usersResponse = await AirtableRepository.ListRecords<AtUser>(
            UserTableName,
            filterByFormula: $"{{Status}} = '{UserStatuses.Active}'"
        );

        if (!usersResponse.Success)
        {
            Logger.LogWarning($"Error getting confirmed users for notification: {usersResponse.AirtableApiError.ErrorMessage}");
            return true; // Event was opened successfully, just notification failed
        }

        var confirmedUsers = usersResponse.Records.Select(r => r.Fields).ToList();
        Logger.LogInformation($"Notifying {confirmedUsers.Count} confirmed users about event opening");

        // Send notification to all confirmed users with localized messages
        var successCount = 0;
        var failCount = 0;

        foreach (var user in confirmedUsers)
        {
            try
            {
                if (user.TelegramChatId != 0)
                {
                    // Get localized event notification message
                    var message = LocalizationService.GetEventOpenNotificationMessage(
                        user.Language, 
                        eventModel.Name, 
                        eventModel.When, 
                        eventModel.Where, 
                        eventModel.Description
                    );
                    
                    await TelegramBotClient.SendMessage(user.TelegramChatId, message, parseMode: ParseMode.Html);
                    successCount++;
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, $"Failed to send event notification to user {user.Name} (ID: {user.RecordId})");
                failCount++;
            }
        }

        Logger.LogInformation($"Event notification completed: {successCount} success, {failCount} failed");
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

        if (eventModel.Status != EventStatuses.BookingOpen)
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

    public async Task<IEnumerable<CatDto>> GetEventCats(string eventRecordId)
    {
        Logger.LogInformation($"Getting cats for event: {eventRecordId}");

        // First, get the event to validate it exists and get the cat IDs
        var eventResponse = await AirtableRepository.RetrieveRecord<AtEvent>(EventTableName, eventRecordId);
        
        if (!eventResponse.Success || eventResponse.Record == null)
        {
            throw new Exception($"Event with ID {eventRecordId} not found.");
        }

        var eventModel = eventResponse.Record.Fields;
        var catIds = eventModel.Cats ?? Array.Empty<string>();

        Logger.LogInformation($"Event '{eventModel.Name}' has {catIds.Length} registered cats");

        if (catIds.Length == 0)
        {
            return Enumerable.Empty<CatDto>();
        }

        // Fetch all cats in parallel for better performance
        var catTasks = catIds.Select(async catId =>
        {
            try
            {
                var catResponse = await AirtableRepository.RetrieveRecord<AtCat>(CatTableName, catId);
                if (catResponse.Success && catResponse.Record != null)
                {
                    return CatConverter.ToDto(catResponse.Record.Fields);
                }
                else
                {
                    Logger.LogWarning($"Could not retrieve cat with ID: {catId}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, $"Error retrieving cat with ID: {catId}");
                return null;
            }
        });

        var cats = await Task.WhenAll(catTasks);
        var validCats = cats.Where(cat => cat != null).Cast<CatDto>().ToList();

        Logger.LogInformation($"Successfully retrieved {validCats.Count} cats for event '{eventModel.Name}'");
        return validCats;
    }
}
