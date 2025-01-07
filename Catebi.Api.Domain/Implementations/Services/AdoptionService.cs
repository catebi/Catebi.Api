using System.Text.Json;
using AirtableApiClient;
using Telegram.Bot;

namespace Catebi.Api.Domain.Implementations.Services;

public class AdoptionService(AirtableBase airtableBase, TelegramBotClient telegramBotClient) : IAdoptionService
{
    private readonly AirtableBase _airtableBase = airtableBase;
    private readonly TelegramBotClient _telegramBotClient = telegramBotClient;

    public async Task<List<AirtableRecord>> GetUserRecords()
    {
        var records = await _airtableBase.ListRecords("user");

        if (records.Success)
        {
            return [..records.Records];
        }

        throw new Exception($"Error fetching Airtable data: {records.AirtableApiError.ErrorMessage}");
    }

    public async Task<bool> ConfirmUser(string recordId)
    {
        // Fetch the user record
        var userRecord = await _airtableBase.RetrieveRecord("user", recordId);

        if (!userRecord.Success || userRecord.Record == null)
        {
            throw new Exception($"User with ID {recordId} not found.");
        }

        var userFields = userRecord.Record.Fields;
        if (!userFields.TryGetValue("telegram_chat_id", out var chatIdElement))
        {
            throw new Exception($"Telegram chat ID missing for user ID {recordId}.");
        }

        var chatId = chatIdElement is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.Number
            ? jsonElement.GetInt64()
            : throw new Exception("Invalid telegram_chat_id format.");

        // Update the user's status to "Confirmed"
        var updatedFields = new Fields();
        updatedFields.AddField("status", "confirmed");
        var updateResponse = await _airtableBase.UpdateRecord("user", updatedFields, recordId);

        if (!updateResponse.Success)
        {
            throw new Exception($"Error updating status for user ID {recordId}: {updateResponse.AirtableApiError.ErrorMessage}");
        }

        // Send a Telegram message
        var userName = userFields.TryGetValue("name", out var value) ? value.ToString() : "User";
        var message = $"Hello {userName}, your account has been confirmed!";
        await _telegramBotClient.SendMessage(chatId, message);

        return true;
    }
}
