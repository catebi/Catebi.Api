using System.Text.Json;
using AirtableApiClient;
using Telegram.Bot;
using Catebi.Api.Domain.Features.AdoptionBot.Enums;
using Catebi.Api.Domain.Features.AdoptionBot.Models;

namespace Catebi.Api.Domain.Features.AdoptionBot;

public class AdoptionBotActionService(
                    AirtableBase AirtableBase,
                    TelegramBotClient TelegramBotClient,
                    ILogger<AdoptionBotActionService> Logger) : IAdoptionBotActionService
{
    private readonly string UserTableName = AirTables.User.ToString();
    private readonly string CatTableName = AirTables.Cat.ToString();
    private readonly string CatPaymentName = AirTables.CatPayment.ToString();
    private readonly string EventTableName = AirTables.Event.ToString();
    private readonly string PaymentOptionTableName = AirTables.PaymentOption.ToString();

    private readonly string StatusColumnName = "Status";

    public async Task<bool> ConfirmUser(string atUserId)
    {
        var userRecord = await AirtableBase.RetrieveRecord<AtUser>(UserTableName, atUserId);

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
        var updateResponse = await AirtableBase.UpdateRecord(UserTableName, updatedFields, atUserId);

        if (!updateResponse.Success)
        {
            throw new Exception($"Error updating status for user ID {atUserId}: {updateResponse.AirtableApiError.ErrorMessage}");
        }

        // Send a Telegram message
        var message = $"Hello {userModel.Name} 👋 \nYour account has been confirmed!";
        await TelegramBotClient.SendMessage(userModel.TelegramChatId, message);

        return true;
    }

    public async Task<bool> AddCatPhoto(string catRecordId, string photoUrl)
    {
        Logger.LogInformation($"Adding cat photo for record ID: {catRecordId}");
        var cat = await AirtableBase.RetrieveRecord<AtCat>(CatTableName, catRecordId);

        Logger.LogInformation($"Cat record retrieved: {catRecordId} - {cat.Success}");

        if (!cat.Success || cat.Record == null)
        {
            throw new Exception($"Cat with ID {catRecordId} not found.");
        }

        var catModel = cat.Record.Fields;

        if (catModel.OwnerTelegramChatId == 0)
        {
            throw new Exception($"Owner Telegram chat ID missing for cat {catModel.Name} (owner: {catModel.OwnerName}) ID {catRecordId}.");
        }

        if (string.IsNullOrEmpty(photoUrl))
        {
            throw new Exception($"❗️File URL is missing for the cat {catModel.Name} (owner: {catModel.OwnerName}, atId {catRecordId}).");
        }

        Logger.LogInformation($"Cat photo validation passed: {catRecordId}");

        // update cat photo status
        var updatedFields = new Fields();

        // Create Attachments list
        var photos = catModel.Photos ?? [];
        var updatedPhotos = photos.Union(
        [
            new AtAttachment
            {
                Url = photoUrl
            }
        ]).Select(x => new AirtableAttachment { Url = x.Url }).ToList();

        updatedFields.AddField("Photos", updatedPhotos);
        var updateResponse = await AirtableBase.UpdateRecord(CatTableName, updatedFields, catRecordId);

        Logger.LogInformation($"Cat photo record created: {updateResponse.Success}");

        if (!updateResponse.Success)
        {
            Logger.LogError($"Error creating cat photo record: {updateResponse.AirtableApiError.ErrorMessage}");
            throw new Exception($"Error updating status for the cat {catModel.Name} (owner: {catModel.OwnerName}, atId {catRecordId}): {updateResponse.AirtableApiError.ErrorMessage}");
        }

        return true;
    }


    public async Task<bool> AddCatPayment(string catRecordId, string imageUrl)
    {
        Logger.LogInformation($"Adding cat payment for record ID: {catRecordId}");
        var cat = await AirtableBase.RetrieveRecord<AtCat>(CatTableName, catRecordId);

        Logger.LogInformation($"Cat record retrieved: {catRecordId} - {cat.Success}");

        if (!cat.Success || cat.Record == null)
        {
            throw new Exception($"Cat with ID {catRecordId} not found.");
        }

        var catModel = cat.Record.Fields;

        if (catModel.OwnerTelegramChatId == 0)
        {
            throw new Exception($"Owner Telegram chat ID missing for cat {catModel.Name} (owner: {catModel.OwnerName}) ID {catRecordId}.");
        }

        if (catModel.Status != CatStatuses.Available)
        {
            throw new Exception($"❗️Cat {catModel.Name} (owner: {catModel.OwnerName}, atId {catRecordId}) is not in ✨Available status.");
        }

        if (string.IsNullOrEmpty(imageUrl))
        {
            throw new Exception($"❗️File URL is missing for the cat {catModel.Name} (owner: {catModel.OwnerName}, atId {catRecordId}).");
        }

        Logger.LogInformation($"Cat payment validation passed: {catRecordId}");

        // update cat payment status
        var updatedFields = new Fields();

        // Create Attachments list
        var attachmentList = new List<AirtableAttachment>
        {
            new() { Url = imageUrl }
        };

        var paymentOptionType = catModel.OwnerIsVolunteer ? PaymentOptions.CatbookVolunteerPrice : PaymentOptions.CatbookStandardPrice;

        // get paymenttype
        var paymentTypes = await AirtableBase.ListRecords<AtPaymentOption>(PaymentOptionTableName, filterByFormula: $"{{Name}}='{paymentOptionType}'");

        if (!paymentTypes.Success || paymentTypes.Records.Count() == 0)
        {
            throw new Exception($"❗️Payment type not found for the cat {catModel.Name} and paymentOption ({paymentOptionType}) (owner: {catModel.OwnerName}, atId {catRecordId}).");
        }

        var paymentTypeId = paymentTypes.Records.First().Id;

        updatedFields.AddField("Cat", new string[] { catRecordId });
        updatedFields.AddField(StatusColumnName, CatPaymentStatuses.ToConfirm.ToString());
        updatedFields.AddField("Proof", attachmentList);
        updatedFields.AddField("PaymentOption", new string[] { paymentTypeId });
        var updateResponse = await AirtableBase.CreateRecord(CatPaymentName, updatedFields);

        Logger.LogInformation($"Cat payment record created: {updateResponse.Success}");

        if (!updateResponse.Success)
        {
            Logger.LogError($"Error creating cat payment record: {updateResponse.AirtableApiError.ErrorMessage}");
            throw new Exception($"Error updating status for the cat {catModel.Name} (owner: {catModel.OwnerName}, atId {catRecordId}): {updateResponse.AirtableApiError.ErrorMessage}");
        }

        // var catPaymentFields = updateResponse.Record.Fields;
        // var catPaymentModel = JsonSerializer.Deserialize<AtCatPayment>(catPaymentFields.ToString(), new JsonSerializerOptions
        // {
        //     PropertyNameCaseInsensitive = true
        // });

        return true;
    }

    public async Task<bool> ConfirmCatPayment(string atCatId)
    {
        var cat = await AirtableBase.RetrieveRecord<AtCat>(CatTableName, atCatId);

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

        // if (catModel.AccountPaymentStatus != CatPaymentStatuses.ToConfirm)
        // {
        //     throw new Exception($"❗️Cat payment for the cat {catModel.Name} (owner: {catModel.OwnerName}, atId {atCatId}) must be in ✨ToConfirm✨ status.");
        // }

        // update cat payment status
        var updatedFields = new Fields();

        updatedFields.AddField(StatusColumnName, CatPaymentStatuses.Confirmed.ToString());
        var updateResponse = await AirtableBase.UpdateRecord(CatPaymentName, updatedFields, catModel.AccountPaymentRecordId);

        if (!updateResponse.Success)
        {
            throw new Exception($"Error updating status for the cat {catModel.Name} (owner: {catModel.OwnerName}, atId {atCatId}): {updateResponse.AirtableApiError.ErrorMessage}");
        }

        // update cat status
        updatedFields = new Fields();
        updatedFields.AddField(StatusColumnName, CatStatuses.AdoptionProcessPaid.ToString());
        updateResponse = await AirtableBase.UpdateRecord(CatTableName, updatedFields, atCatId);

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

    public Task<bool> ConfirmCatbookPlacement(string atCatId) => throw new NotImplementedException();
    public Task<bool> OpenEventRegistration(string atEventId) => throw new NotImplementedException();
    public Task<bool> CloseEventRegistration(string atEventId) => throw new NotImplementedException();
}
