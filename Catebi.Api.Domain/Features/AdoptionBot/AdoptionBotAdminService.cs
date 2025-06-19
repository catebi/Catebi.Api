using AirtableApiClient;
using Telegram.Bot;
using Catebi.Api.Domain.Features.AdoptionBot.Enums;
using Catebi.Api.Domain.Features.AdoptionBot.Models;

namespace Catebi.Api.Domain.Features.AdoptionBot;

public class AdoptionBotAdminService(
    IAirtableRepository AirtableRepository,
    TelegramBotClient TelegramBotClient,
    ILogger<AdoptionBotAdminService> Logger) : IAdoptionBotAdminService
{
    private readonly string UserTableName = AirTables.User.ToString();
    private readonly string CatTableName = AirTables.Cat.ToString();
    private readonly string CatPaymentName = AirTables.CatPayment.ToString();
    private readonly string StatusColumnName = "Status";

    public async Task<bool> ConfirmUser(string atUserId)
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
        updatedFields.AddField(StatusColumnName, UserStatuses.Confirmed.ToString());
        var updateResponse = await AirtableRepository.UpdateRecord(UserTableName, updatedFields, atUserId);

        if (!updateResponse.Success)
        {
            throw new Exception($"Error updating status for user ID {atUserId}: {updateResponse.AirtableApiError.ErrorMessage}");
        }

        // Send a Telegram message
        var message = $"Hello {userModel.Name} 👋 \nYour account has been confirmed!";
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
} 