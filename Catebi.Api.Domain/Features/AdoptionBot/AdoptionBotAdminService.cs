using AirtableApiClient;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Catebi.Api.Domain.Features.AdoptionBot.Enums;
using Catebi.Api.Domain.Features.AdoptionBot.Converters;

namespace Catebi.Api.Domain.Features.AdoptionBot;

public class AdoptionBotAdminService(
    IAirtableRepository AirtableRepository,
    TelegramBotClient TelegramBotClient,
    ILogger<AdoptionBotAdminService> Logger) : IAdoptionBotAdminService
{
    private readonly string UserTableName = AirTables.User.ToString();
    private readonly string CatTableName = AirTables.Cat.ToString();
    private readonly string CatPaymentName = AirTables.CatPayment.ToString();
    private readonly string MessageTableName = AirTables.Message.ToString();
    private readonly string StatusColumnName = "Status";

    public async Task<bool> ConfirmUser(string atUserId, bool isVolunteer, string? notes)
    {
        var userRecord = await AirtableRepository.RetrieveRecord<AtUser>(UserTableName, atUserId);

        if (!userRecord.Success || userRecord.Record == null)
        {
            throw new Exception($"User with ID {atUserId} not found.");
        }

        var userModel = userRecord.Record.Fields;

        if (userModel.TelegramChatId == 0)
        {
            throw new Exception($"Telegram chat ID missing for user ID {atUserId}.");
        }

        if (userModel.Status != UserStatuses.ToConfirm)
        {
            throw new Exception($"User ID {atUserId} must be in ToConfirm status.");
        }

        var updatedFields = new Fields();
        updatedFields.AddField(StatusColumnName, UserStatuses.Active.ToString());
        updatedFields.AddField("IsVolunteer", isVolunteer);
        if (!string.IsNullOrEmpty(notes))
        {
            updatedFields.AddField("Notes", notes);
        }

        var updateResponse = await AirtableRepository.UpdateRecord(UserTableName, updatedFields, atUserId);

        if (!updateResponse.Success)
        {
            throw new Exception($"Error updating user ID {atUserId}: {updateResponse.AirtableApiError.ErrorMessage}");
        }

        // Send a Telegram message
        var volunteerStatus = isVolunteer ? "as a volunteer" : "as a cat owner";
        var message = $"Hello {userModel.Name} 👋\nYour account has been confirmed {volunteerStatus}! Welcome to our community!";
        await TelegramBotClient.SendMessage(userModel.TelegramChatId, message);

        return true;
    }

    public async Task<bool> ConfirmCatPayment(string atCatId)
    {
        var cat = await AirtableRepository.RetrieveRecord<AtCat>(CatTableName, atCatId);

        if (!cat.Success || cat.Record == null)
        {
            throw new Exception($"Cat with ID {atCatId} not found.");
        }

        // Update the user's status or grant paid features
        var catModel = cat.Record.Fields;

        if (catModel.OwnerTelegramChatId == 0)
        {
            throw new Exception($"Owner Telegram chat ID missing for cat {catModel.Name} (owner: {catModel.OwnerName}) ID {atCatId}.");
        }

        if (catModel.Status != CatStatuses.Available)
        {
            throw new Exception($"❗️Cat {catModel.Name} (owner: {catModel.OwnerName}, atId {atCatId}) is not in ✨Available✨ status.");
        }

        if (catModel.AccountPaymentRecordId == null || catModel.AccountPaymentType != PaymentOptionTypes.Account)
        {
            throw new Exception($"❗️Cat payment info not found for the cat {catModel.Name} (owner: {catModel.OwnerName}, atId {atCatId}).");
        }

        // update cat payment status
        var updatedFields = new Fields();

        updatedFields.AddField(StatusColumnName, CatPaymentStatuses.Confirmed.ToString());
        var updateResponse = await AirtableRepository.UpdateRecord(CatPaymentName, updatedFields, catModel.AccountPaymentRecordId);

        if (!updateResponse.Success)
        {
            throw new Exception($"Error updating status for the cat {catModel.Name} (owner: {catModel.OwnerName}, atId {atCatId}): {updateResponse.AirtableApiError.ErrorMessage}");
        }

        // update cat status
        updatedFields = new Fields();
        updatedFields.AddField(StatusColumnName, CatStatuses.AdoptionProcessPaid.ToString());
        updateResponse = await AirtableRepository.UpdateRecord(CatTableName, updatedFields, atCatId);

        if (!updateResponse.Success)
        {
            throw new Exception($"Error updating status for cat {catModel.Name} (owner: {catModel.OwnerName}, atId {atCatId}): {updateResponse.AirtableApiError.ErrorMessage}");
        }

        // Send a Telegram message
        var message = @$"
Hello {catModel.OwnerName} 👋
Payment for your cat {catModel.Name} has been confirmed. Congrats!

You can now access to push your cat to the Catbook or to book event for them.";

        await TelegramBotClient.SendMessage(catModel.OwnerTelegramChatId, message);

        return true;
    }

    public async Task<MessageDto> BroadcastMessage(string content, string adminRecordId)
    {
        Logger.LogInformation("Broadcasting message to all confirmed users");

        // Create message record first
        var fields = new Fields();
        fields.AddField("Content", content);
        fields.AddField("Admin", new string[] { adminRecordId });

        var createResponse = await AirtableRepository.CreateRecord(MessageTableName, fields);

        if (!createResponse.Success)
        {
            throw new Exception($"Error creating message record: {createResponse.AirtableApiError.ErrorMessage}");
        }

        // Get all confirmed users
        var usersResponse = await AirtableRepository.ListRecords<AtUser>(
            UserTableName,
            filterByFormula: $"{{Status}} = '{UserStatuses.Active}'"
        );

        if (!usersResponse.Success)
        {
            throw new Exception($"Error getting confirmed users: {usersResponse.AirtableApiError.ErrorMessage}");
        }

        var confirmedUsers = usersResponse.Records.Select(r => r.Fields).ToList();
        Logger.LogInformation($"Found {confirmedUsers.Count} confirmed users for broadcast");

        // Send message to all confirmed users
        var successCount = 0;
        var failCount = 0;

        foreach (var user in confirmedUsers)
        {
            try
            {
                if (user.TelegramChatId != 0)
                {
                    await TelegramBotClient.SendMessage(user.TelegramChatId, content, parseMode: ParseMode.Html);
                    successCount++;
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, $"Failed to send message to user {user.Name} (ID: {user.RecordId})");
                failCount++;
            }
        }

        Logger.LogInformation($"Broadcast completed: {successCount} success, {failCount} failed");

        // Retrieve the created message to return
        var messageRecord = await AirtableRepository.RetrieveRecord<AtMessage>(MessageTableName, createResponse.Record.Id);
        if (!messageRecord.Success || messageRecord.Record == null)
        {
            throw new Exception($"Error retrieving created message: {createResponse.AirtableApiError.ErrorMessage}");
        }

        return MessageConverter.ToDto(messageRecord.Record.Fields);
    }

    public async Task<IEnumerable<MessageDto>> GetBroadcastMessages()
    {
        Logger.LogInformation("Getting all broadcast messages");

        var response = await AirtableRepository.ListRecords<AtMessage>(MessageTableName);

        if (!response.Success)
        {
            throw new Exception($"Error getting messages: {response.AirtableApiError.ErrorMessage}");
        }

        var messages = response.Records.Select(r => MessageConverter.ToDto(r.Fields)).ToList();
        Logger.LogInformation($"Found {messages.Count} broadcast messages");
        return messages;
    }

    public async Task<IEnumerable<UserDto>> GetUsersToConfirm()
    {
        Logger.LogInformation("Getting users with ToConfirm status");

        var response = await AirtableRepository.ListRecords<AtUser>(
            UserTableName,
            filterByFormula: $"{{Status}} = '{UserStatuses.ToConfirm}'"
        );

        if (!response.Success)
        {
            throw new Exception($"Error getting users to confirm: {response.AirtableApiError.ErrorMessage}");
        }

        var users = response.Records.Select(r => UserConverter.ToDto(r.Fields)).ToList();
        Logger.LogInformation($"Found {users.Count} users to confirm");
        return users;
    }
}
