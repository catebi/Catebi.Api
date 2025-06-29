using AirtableApiClient;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Catebi.Api.Domain.Features.AdoptionBot.Enums;
using Catebi.Api.Domain.Features.AdoptionBot.Models;
using Catebi.Api.Domain.Features.AdoptionBot.ViewModels;
using Catebi.Api.Domain.Features.AdoptionBot.Converters;
using Catebi.Api.Domain.Contracts.Services;

namespace Catebi.Api.Domain.Features.AdoptionBot;

public class AdoptionBotAdminService(
    IAirtableRepository AirtableRepository,
    TelegramBotClient TelegramBotClient,
    ILocalizationService LocalizationService,
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

        // Send a localized Telegram message
        var message = LocalizationService.GetUserConfirmationMessage(userModel.Language, userModel.Name, isVolunteer);
        await TelegramBotClient.SendMessage(userModel.TelegramChatId, message);

        return true;
    }

    public async Task<bool> ConfirmCatPayment(string paymentRecordId)
    {
        // First get the payment record
        var paymentRecord = await AirtableRepository.RetrieveRecord<AtCatPayment>(CatPaymentName, paymentRecordId);

        if (!paymentRecord.Success || paymentRecord.Record == null)
        {
            throw new Exception($"Payment with ID {paymentRecordId} not found.");
        }

        var paymentModel = paymentRecord.Record.Fields;

        if (paymentModel.Status != CatPaymentStatuses.ToConfirm)
        {
            throw new Exception($"Payment {paymentRecordId} must be in ToConfirm status.");
        }

        // Get the associated cat record
        var catRecordId = paymentModel.CatRecordId;
        if (string.IsNullOrEmpty(catRecordId))
        {
            throw new Exception($"Cat record ID missing for payment {paymentRecordId}.");
        }

        var cat = await AirtableRepository.RetrieveRecord<AtCat>(CatTableName, catRecordId);

        if (!cat.Success || cat.Record == null)
        {
            throw new Exception($"Cat with ID {catRecordId} not found for payment {paymentRecordId}.");
        }

        var catModel = cat.Record.Fields;

        if (catModel.OwnerTelegramChatId == 0)
        {
            throw new Exception($"Owner Telegram chat ID missing for cat {catModel.Name} (owner: {catModel.OwnerName}) ID {catRecordId}.");
        }

        if (catModel.Status != CatStatuses.SearchingForHome)
        {
            throw new Exception($"❗️Cat {catModel.Name} (owner: {catModel.OwnerName}, catId {catRecordId}) is not in ✨SearchingForHome✨ status.");
        }

        // Update payment status to confirmed
        var updatedFields = new Fields();
        updatedFields.AddField(StatusColumnName, CatPaymentStatuses.Confirmed.ToString());
        var updateResponse = await AirtableRepository.UpdateRecord(CatPaymentName, updatedFields, paymentRecordId);

        if (!updateResponse.Success)
        {
            throw new Exception($"Error updating payment status for payment {paymentRecordId}: {updateResponse.AirtableApiError.ErrorMessage}");
        }

        // Update cat status (keep as SearchingForHome since payment is now confirmed)
        updatedFields = new Fields();
        updatedFields.AddField(StatusColumnName, CatStatuses.SearchingForHome.ToString());
        updateResponse = await AirtableRepository.UpdateRecord(CatTableName, updatedFields, catRecordId);

        if (!updateResponse.Success)
        {
            throw new Exception($"Error updating cat status for cat {catModel.Name} (catId: {catRecordId}): {updateResponse.AirtableApiError.ErrorMessage}");
        }

        // Get owner's language for localized message
        var ownerRecord = await AirtableRepository.RetrieveRecord<AtUser>(UserTableName, catModel.OwnerRecordId!);
        var ownerLanguage = Languages.ru;

        if (ownerRecord.Success && ownerRecord.Record != null)
        {
            ownerLanguage = ownerRecord.Record.Fields.Language;
        }

        // Send a localized Telegram message
        var message = LocalizationService.GetCatPaymentConfirmationMessage(ownerLanguage, catModel.OwnerName!, catModel.Name);
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
                    // For broadcast messages, use the original content (admin can write in any language)
                    var localizedContent = LocalizationService.GetBroadcastMessage(user.Language, content);
                    await TelegramBotClient.SendMessage(user.TelegramChatId, localizedContent, parseMode: ParseMode.Html);
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

    public async Task<IEnumerable<CatPaymentDto>> GetPaymentsToConfirm()
    {
        Logger.LogInformation("Getting cat payments with ToConfirm status");

        var response = await AirtableRepository.ListRecords<AtCatPayment>(
            CatPaymentName,
            filterByFormula: $"{{Status}} = '{CatPaymentStatuses.ToConfirm}'"
        );

        if (!response.Success)
        {
            throw new Exception($"Error getting payments to confirm: {response.AirtableApiError.ErrorMessage}");
        }

        var payments = response.Records.Select(r => CatPaymentConverter.ToDto(r.Fields)).ToList();
        Logger.LogInformation($"Found {payments.Count} payments to confirm");
        return payments;
    }
}
