using AirtableApiClient;
using Telegram.Bot;
using Catebi.Api.Domain.Features.AdoptionBot.Enums;
using Catebi.Api.Domain.Features.AdoptionBot.Models;
using Catebi.Api.Domain.Features.AdoptionBot.Converters;

namespace Catebi.Api.Domain.Features.AdoptionBot;

public class AdoptionBotCatService(
    IAirtableRepository AirtableRepository,
    TelegramBotClient TelegramBotClient,
    ILogger<AdoptionBotCatService> Logger) : IAdoptionBotCatService
{
    private readonly string CatTableName = AirTables.Cat.ToString();
    private readonly string CatPaymentName = AirTables.CatPayment.ToString();
    private readonly string PaymentOptionTableName = AirTables.PaymentOption.ToString();
    private readonly string StatusColumnName = "Status";

    public async Task<CatDto> AddCat(CatDto catDto)
    {
        Logger.LogInformation($"Adding new cat: {catDto.Name}");

        var fields = new Fields();
        fields.AddField("Name", catDto.Name);
        fields.AddField("DateOfBirth", DateTime.Parse(catDto.DateOfBirth));
        fields.AddField("Status", catDto.Status);
        fields.AddField("Owner", new string[] { catDto.OwnerRecordId });

        // Add vaccination fields if provided
        if (catDto.IsVaccinatedComplex.HasValue)
        {
            fields.AddField("IsVaccinatedComplex", catDto.IsVaccinatedComplex.Value);
        }
        if (catDto.IsVaccinatedRabies.HasValue)
        {
            fields.AddField("IsVaccinatedRabies", catDto.IsVaccinatedRabies.Value);
        }

        if (catDto.MainPhoto != null && !string.IsNullOrEmpty(catDto.MainPhoto.Url))
        {
            Logger.LogInformation($"Adding main photo for cat: {catDto.Name}");
            var mainPhotoAttachment = new AirtableAttachment { Url = catDto.MainPhoto.Url };
            fields.AddField("MainPhoto", new List<AirtableAttachment> { mainPhotoAttachment });
        }

        var response = await AirtableRepository.CreateRecord(CatTableName, fields);

        if (!response.Success)
        {
            Logger.LogError($"Error adding cat: {response.AirtableApiError.ErrorMessage}");
            throw new Exception($"Error adding cat: {response.AirtableApiError.ErrorMessage}");
        }

        var createdCat = await GetCatById(response.Record.Id);
        Logger.LogInformation($"Cat added successfully: {catDto.Name}");
        return createdCat!;
    }

    public async Task<IEnumerable<CatDto>> GetCatsByUserId(string userId)
    {
        Logger.LogInformation($"Getting cats for user: {userId}");

        var response = await AirtableRepository.ListRecords<AtCat>(
            CatTableName,
            filterByFormula: $"{{OwnerRecordId}} = '{userId}'"
        );

        if (!response.Success)
        {
            Logger.LogError($"Error getting cats: {response.AirtableApiError.ErrorMessage}");
            throw new Exception($"Error getting cats: {response.AirtableApiError.ErrorMessage}");
        }

        var cats = response.Records.Select(r => CatConverter.ToDto(r.Fields)).ToList();
        Logger.LogInformation($"Found {cats.Count} cats for user {userId}");
        return cats;
    }

    public async Task<CatDto?> GetCatById(string recordId)
    {
        Logger.LogInformation($"Getting cat by ID: {recordId}");

        var response = await AirtableRepository.RetrieveRecord<AtCat>(CatTableName, recordId);

        if (!response.Success)
        {
            Logger.LogError($"Error getting cat: {response.AirtableApiError.ErrorMessage}");
            throw new Exception($"Error getting cat: {response.AirtableApiError.ErrorMessage}");
        }

        if (response.Record == null)
        {
            Logger.LogInformation($"No cat found with ID: {recordId}");
            return null;
        }

        Logger.LogInformation($"Found cat: {response.Record.Fields.Name}");
        return CatConverter.ToDto(response.Record.Fields);
    }

    public async Task<CatDto> UpdateCat(CatDto catDto)
    {
        if (string.IsNullOrEmpty(catDto.RecordId))
        {
            throw new ArgumentException("Record ID is required for updating a cat");
        }

        Logger.LogInformation($"Updating cat: {catDto.Name}");

        var fields = new Fields();
        fields.AddField("Name", catDto.Name);
        fields.AddField("DateOfBirth", DateTime.Parse(catDto.DateOfBirth));
        fields.AddField("Status", catDto.Status);

        // Add vaccination fields if provided
        if (catDto.IsVaccinatedComplex.HasValue)
        {
            fields.AddField("IsVaccinatedComplex", catDto.IsVaccinatedComplex.Value);
        }
        if (catDto.IsVaccinatedRabies.HasValue)
        {
            fields.AddField("IsVaccinatedRabies", catDto.IsVaccinatedRabies.Value);
        }
        if (!string.IsNullOrEmpty(catDto.OwnerNotes))
        {
            fields.AddField("OwnerNotes", catDto.OwnerNotes);
        }

        // Handle main photo if provided
        if (catDto.MainPhoto != null && !string.IsNullOrEmpty(catDto.MainPhoto.Url))
        {
            Logger.LogInformation($"Updating main photo for cat: {catDto.Name}");
            var mainPhotoAttachment = new AirtableAttachment { Url = catDto.MainPhoto.Url };
            fields.AddField("MainPhoto", new List<AirtableAttachment> { mainPhotoAttachment });
        }

        var response = await AirtableRepository.UpdateRecord(CatTableName, fields, catDto.RecordId);

        if (!response.Success)
        {
            Logger.LogError($"Error updating cat: {response.AirtableApiError.ErrorMessage}");
            throw new Exception($"Error updating cat: {response.AirtableApiError.ErrorMessage}");
        }

        Logger.LogInformation($"Cat updated successfully: {catDto.Name}");
        return await GetCatById(catDto.RecordId) ?? catDto;
    }

    public async Task<IEnumerable<CatPaymentDto>> GetCatPayments(string catId)
    {
        Logger.LogInformation($"Getting payments for cat: {catId}");

        var response = await AirtableRepository.ListRecords<AtCatPayment>(
            CatPaymentName,
            filterByFormula: $"{{CatRecordId}} = '{catId}'"
        );

        if (!response.Success)
        {
            Logger.LogError($"Error getting cat payments: {response.AirtableApiError.ErrorMessage}");
            throw new Exception($"Error getting cat payments: {response.AirtableApiError.ErrorMessage}");
        }

        var payments = response.Records.Select(r => CatPaymentConverter.ToDto(r.Fields)).ToList();
        Logger.LogInformation($"Found {payments.Count} payments for cat {catId}");
        return payments;
    }

    public async Task<bool> AddCatPhoto(string catRecordId, string photoUrl)
    {
        Logger.LogInformation($"Adding cat photo for record ID: {catRecordId}");
        var cat = await AirtableRepository.RetrieveRecord<AtCat>(CatTableName, catRecordId);

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
        var updateResponse = await AirtableRepository.UpdateRecord(CatTableName, updatedFields, catRecordId);

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
        var cat = await AirtableRepository.RetrieveRecord<AtCat>(CatTableName, catRecordId);

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
        var paymentTypes = await AirtableRepository.ListRecords<AtPaymentOption>(PaymentOptionTableName, filterByFormula: $"{{Name}}='{paymentOptionType}'");

        if (!paymentTypes.Success || paymentTypes.Records.Count() == 0)
        {
            throw new Exception($"❗️Payment type not found for the cat {catModel.Name} and paymentOption ({paymentOptionType}) (owner: {catModel.OwnerName}, atId {catRecordId}).");
        }

        var paymentTypeId = paymentTypes.Records.First().Id;

        updatedFields.AddField("Cat", new string[] { catRecordId });
        updatedFields.AddField(StatusColumnName, CatPaymentStatuses.ToConfirm.ToString());
        updatedFields.AddField("Proof", attachmentList);
        updatedFields.AddField("PaymentOption", new string[] { paymentTypeId });
        var updateResponse = await AirtableRepository.CreateRecord(CatPaymentName, updatedFields);

        Logger.LogInformation($"Cat payment record created: {updateResponse.Success}");

        if (!updateResponse.Success)
        {
            Logger.LogError($"Error creating cat payment record: {updateResponse.AirtableApiError.ErrorMessage}");
            throw new Exception($"Error updating status for the cat {catModel.Name} (owner: {catModel.OwnerName}, atId {catRecordId}): {updateResponse.AirtableApiError.ErrorMessage}");
        }

        return true;
    }
}
