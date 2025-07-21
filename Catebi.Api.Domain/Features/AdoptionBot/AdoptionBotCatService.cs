using AirtableApiClient;
using Telegram.Bot;
using Catebi.Api.Domain.Features.AdoptionBot.Enums;
using Catebi.Api.Domain.Features.AdoptionBot.Converters;
using Catebi.Api.Domain.Contracts.Services;


namespace Catebi.Api.Domain.Features.AdoptionBot;

public class AdoptionBotCatService(
    IAirtableRepository AirtableRepository,
    TelegramBotClient TelegramBotClient,
    ILocalizationService LocalizationService,
    IAdoptionBotAdminService AdminService,
    ILogger<AdoptionBotCatService> Logger,
    AirtableBase Airtable) : IAdoptionBotCatService
{
    private readonly string UserTableName = AirTables.User.ToString();
    private readonly string CatTableName = AirTables.Cat.ToString();
    private readonly string CatPaymentName = AirTables.CatPayment.ToString();
    private readonly string PaymentOptionTableName = AirTables.PaymentOption.ToString();
    private readonly string EventTableName = AirTables.Event.ToString();
    private readonly string StatusColumnName = "Status";

    public async Task<CatDto> AddCat(CatDto catDto)
    {
        Logger.LogInformation($"Adding new cat: {catDto.Name}");

        // Validate that the user (owner) is confirmed before allowing cat creation
        var userRecord = await AirtableRepository.RetrieveRecord<AtUser>(UserTableName, catDto.OwnerRecordId);

        if (!userRecord.Success || userRecord.Record == null)
        {
            throw new Exception($"User with ID {catDto.OwnerRecordId} not found.");
        }

        var userModel = userRecord.Record.Fields;

        if (userModel.Status != UserStatuses.Active)
        {
            throw new Exception($"❗️ User {userModel.Name} must be confirmed by an admin before creating cats. Current status: {userModel.StatusValue}");
        }

        Logger.LogInformation($"User validation passed: {userModel.Name} is confirmed (Active)");

        var fields = new Fields();
        fields.AddField("Name", catDto.Name);
        fields.AddField("Sex", catDto.Sex);
        fields.AddField("DateOfBirth", DateTime.Parse(catDto.DateOfBirth));
        fields.AddField("Status", CatStatuses.SearchingForHome.ToString());
        fields.AddField("Owner", new string[] { catDto.OwnerRecordId });
        fields.AddField("IsCatebiCat", catDto.IsCatebiCat);

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

        var paymentOptionType = response.Record.Fields.OwnerIsVolunteer
            ? PaymentOptions.CatbookVolunteerPrice
            : PaymentOptions.CatbookStandardPrice;
        var paymentOptions = await AirtableRepository.ListRecords<AtPaymentOption>(PaymentOptionTableName, filterByFormula: $"{{Name}}='{paymentOptionType}'");

        if (!paymentOptions.Success || paymentOptions.Records.Count() == 0)
        {
            throw new Exception($"❗️Payment type not found for the cat {response.Record.Fields.Name} and paymentOption ({paymentOptionType}) (owner: {response.Record.Fields.OwnerName}, atId {recordId}).");
        }

        Logger.LogInformation($"Found cat: {response.Record.Fields.Name}");
        return CatConverter.ToDto(response.Record.Fields, paymentOptions.Records.First().Fields.Price);
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
        fields.AddField("Sex", catDto.Sex);
        fields.AddField("DateOfBirth", DateTime.Parse(catDto.DateOfBirth));
        fields.AddField("Status", catDto.Status);
        fields.AddField("IsCatebiCat", catDto.IsCatebiCat);

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

    public async Task<IEnumerable<CatPaymentDto>> GetCatPayments(string catRecordId)
    {
        Logger.LogInformation($"Getting payments for cat: {catRecordId}");

        var response = await AirtableRepository.ListRecords<AtCatPayment>(
            CatPaymentName,
            filterByFormula: $"{{CatRecordId}} = '{catRecordId}'"
        );

        if (!response.Success)
        {
            Logger.LogError($"Error getting cat payments: {response.AirtableApiError.ErrorMessage}");
            throw new Exception($"Error getting cat payments: {response.AirtableApiError.ErrorMessage}");
        }

        var payments = response.Records.Select(r => CatPaymentConverter.ToDto(r.Fields)).ToList();
        Logger.LogInformation($"Found {payments.Count} payments for cat {catRecordId}");
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

    public async Task<CatPaymentDto> AddCatPayment(string catRecordId, string imageUrl)
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

        if (catModel.Status != CatStatuses.SearchingForHome)
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

        var paymentRecord = await AirtableRepository.RetrieveRecord<AtCatPayment>(CatPaymentName, updateResponse.Record.Id);
        if (!paymentRecord.Success || paymentRecord.Record == null)
        {
            throw new Exception($"Error retrieving payment record for cat {catModel.Name} (owner: {catModel.OwnerName}, atId {catRecordId}): {paymentRecord.AirtableApiError.ErrorMessage}");
        }

        var paymentDto = CatPaymentConverter.ToDto(paymentRecord.Record.Fields);

        // Notify admins about new payment submission
        try
        {
            await AdminService.NotifyAdminsAboutPaymentSubmission(
                catModel.Name,
                catModel.OwnerName!,
                catRecordId,
                updateResponse.Record.Id);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, $"Failed to notify admins about payment submission for cat {catModel.Name}");
            // Don't throw here - payment submission was successful, notification failure shouldn't fail the submission
        }

        return paymentDto;
    }

    public async Task<bool> RegisterCatToEvent(string catRecordId, string eventRecordId)
    {
        Logger.LogInformation($"Registering cat {catRecordId} to event {eventRecordId}");

        // Validate cat exists
        var catResponse = await AirtableRepository.RetrieveRecord<AtCat>(CatTableName, catRecordId);
        if (!catResponse.Success || catResponse.Record == null)
        {
            throw new Exception($"Cat with ID {catRecordId} not found.");
        }

        var catModel = catResponse.Record.Fields;
        Logger.LogInformation($"Cat found: {catModel.Name} (Owner: {catModel.OwnerName})");

        // Validate event exists and get current state
        var eventResponse = await AirtableRepository.RetrieveRecord<AtEvent>(EventTableName, eventRecordId);
        if (!eventResponse.Success || eventResponse.Record == null)
        {
            throw new Exception($"Event with ID {eventRecordId} not found.");
        }

        var eventModel = eventResponse.Record.Fields;
        Logger.LogInformation($"Event found: {eventModel.Name}");

        // Validate event is open for registration
        if (eventModel.Status != EventStatuses.BookingOpen)
        {
            throw new Exception($"Event '{eventModel.Name}' is not open for registration. Current status: {eventModel.Status}");
        }

        // Check if cat is already registered
        var currentCats = eventModel.Cats ?? Array.Empty<string>();
        if (currentCats.Contains(catRecordId))
        {
            throw new Exception($"Cat '{catModel.Name}' is already registered for event '{eventModel.Name}'");
        }

        // Check if there are available slots
        var currentCatCount = eventModel.Cats.Count();
        var maxCatSlots = eventModel.PaidSlotCount + eventModel.FreeSlotCount;

        if (currentCatCount >= maxCatSlots)
        {
            throw new Exception($"Event '{eventModel.Name}' is full. No available slots (current: {currentCatCount}/{maxCatSlots})");
        }

        // Add cat to event
        var updatedCats = currentCats.Append(catRecordId).ToArray();
        var updatedFields = new Fields();
        updatedFields.AddField("Cats", updatedCats);

        var updateResponse = await AirtableRepository.UpdateRecord(EventTableName, updatedFields, eventRecordId);

        if (!updateResponse.Success)
        {
            Logger.LogError($"Error registering cat to event: {updateResponse.AirtableApiError.ErrorMessage}");
            throw new Exception($"Error registering cat '{catModel.Name}' to event '{eventModel.Name}': {updateResponse.AirtableApiError.ErrorMessage}");
        }

        Logger.LogInformation($"Successfully registered cat '{catModel.Name}' to event '{eventModel.Name}'");

        // Optional: Send notification to cat owner
        if (catModel.OwnerTelegramChatId != 0)
        {
            try
            {
                var message = $"🎉 Great news! Your cat '{catModel.Name}' has been successfully registered for the event '{eventModel.Name}' on {eventModel.When:yyyy-MM-dd} at {eventModel.Where}.";
                await TelegramBotClient.SendMessage(catModel.OwnerTelegramChatId, message);
                Logger.LogInformation($"Notification sent to cat owner: {catModel.OwnerName}");
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, $"Failed to send notification to cat owner {catModel.OwnerName}");
                // Don't throw here, registration was successful
            }
        }

        return true;
    }

    public async Task<bool> RegisterCatToEvent(CatToEventRequest request)
    {
        Logger.LogInformation($"Registering cat {request.CatRecordId} to event {request.EventRecordId} by user {request.UserRecordId}");

        // Validate cat exists
        var catResponse = await AirtableRepository.RetrieveRecord<AtCat>(CatTableName, request.CatRecordId);
        if (!catResponse.Success || catResponse.Record == null)
        {
            throw new Exception($"Cat with ID {request.CatRecordId} not found.");
        }

        var catModel = catResponse.Record.Fields;
        Logger.LogInformation($"Cat found: {catModel.Name} (Owner: {catModel.OwnerName})");

        // Validate user exists and get user details
        var userResponse = await AirtableRepository.RetrieveRecord<AtUser>(UserTableName, request.UserRecordId);
        if (!userResponse.Success || userResponse.Record == null)
        {
            throw new Exception($"User with ID {request.UserRecordId} not found.");
        }

        var userModel = userResponse.Record.Fields;
        Logger.LogInformation($"User found: {userModel.Name} ({userModel.Telegram}) - Role: {userModel.Role}");

        // Validate user permissions: must be the owner OR have Admin role
        var isOwner = catModel.OwnerRecordId == request.UserRecordId;
        var isAdmin = userModel.Role == UserRoles.Admin;

        if (!isOwner && !isAdmin)
        {
            throw new Exception($"Access denied. User '{userModel.Name}' ({userModel.Telegram}) is not the owner of cat '{catModel.Name}' and does not have Admin role. " +
                $"Only the cat owner or admins can register a cat to an event.");
        }

        Logger.LogInformation($"Permission granted: User '{userModel.Name}' is {(isOwner ? "the owner" : "an admin")}");

        // Validate event exists and get current state
        var eventResponse = await AirtableRepository.RetrieveRecord<AtEvent>(EventTableName, request.EventRecordId);
        if (!eventResponse.Success || eventResponse.Record == null)
        {
            throw new Exception($"Event with ID {request.EventRecordId} not found.");
        }

        var eventModel = eventResponse.Record.Fields;
        Logger.LogInformation($"Event found: {eventModel.Name}");

        // Validate event is open for registration
        if (eventModel.Status != EventStatuses.BookingOpen)
        {
            throw new Exception($"Event '{eventModel.Name}' is not open for registration. Current status: {eventModel.Status}");
        }

        // Check if cat is already registered
        var currentCats = eventModel.Cats ?? Array.Empty<string>();
        if (currentCats.Contains(request.CatRecordId))
        {
            throw new Exception($"Cat '{catModel.Name}' is already registered for event '{eventModel.Name}'");
        }

        // Check if there are available slots
        var currentCatCount = eventModel.Cats.Count();
        var maxCatSlots = eventModel.PaidSlotCount + eventModel.FreeSlotCount;

        if (currentCatCount >= maxCatSlots)
        {
            throw new Exception($"Event '{eventModel.Name}' is full. No available slots (current: {currentCatCount}/{maxCatSlots})");
        }

        // Add cat to event
        var updatedCats = currentCats.Append(request.CatRecordId).ToArray();
        var updatedFields = new Fields();
        updatedFields.AddField("Cats", updatedCats);

        var updateResponse = await AirtableRepository.UpdateRecord(EventTableName, updatedFields, request.EventRecordId);

        if (!updateResponse.Success)
        {
            Logger.LogError($"Error registering cat to event: {updateResponse.AirtableApiError.ErrorMessage}");
            throw new Exception($"Error registering cat '{catModel.Name}' to event '{eventModel.Name}': {updateResponse.AirtableApiError.ErrorMessage}");
        }

        Logger.LogInformation($"Successfully registered cat '{catModel.Name}' to event '{eventModel.Name}' by user '{userModel.Name}'");

        // Optional: Send notification to cat owner
        if (catModel.OwnerTelegramChatId != 0)
        {
            try
            {
                // Use the owner's language for localization
                var ownerLanguage = Languages.en;
                var ownerResponse = await AirtableRepository.RetrieveRecord<AtUser>(UserTableName, catModel.OwnerRecordId);
                if (ownerResponse.Success && ownerResponse.Record != null)
                {
                    ownerLanguage = ownerResponse.Record.Fields.Language;
                }
                var message = LocalizationService.GetCatRegisteredToEventMessage(ownerLanguage, catModel.Name, eventModel.Name, eventModel.When, eventModel.Where);
                await TelegramBotClient.SendMessage(catModel.OwnerTelegramChatId, message);
                Logger.LogInformation($"Notification sent to cat owner: {catModel.OwnerName}");
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, $"Failed to send notification to cat owner {catModel.OwnerName}");
                // Don't throw here, registration was successful
            }
        }
        // Send admin notification
        try
        {
            await AdminService.NotifyAdminsAboutCatRegisteredToEvent(
                catModel.Name,
                catModel.OwnerName!,
                catModel.RecordId!,
                eventModel.Name,
                eventModel.RecordId!,
                userModel.Name,
                userModel.Telegram
            );
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, $"Failed to notify admins about cat registration to event for {catModel.Name}");
        }

        return true;
    }

    public async Task<bool> RemoveCatFromEvent(CatToEventRequest request)
    {
        Logger.LogInformation($"Excluding cat {request.CatRecordId} from event {request.EventRecordId} by user {request.UserRecordId}");

        // Validate cat exists
        var catResponse = await AirtableRepository.RetrieveRecord<AtCat>(CatTableName, request.CatRecordId);
        if (!catResponse.Success || catResponse.Record == null)
        {
            throw new Exception($"Cat with ID {request.CatRecordId} not found.");
        }

        var catModel = catResponse.Record.Fields;
        Logger.LogInformation($"Cat found: {catModel.Name} (Owner: {catModel.OwnerName})");

        // Validate user exists and get user details
        var userResponse = await AirtableRepository.RetrieveRecord<AtUser>(UserTableName, request.UserRecordId);
        if (!userResponse.Success || userResponse.Record == null)
        {
            throw new Exception($"User with ID {request.UserRecordId} not found.");
        }

        var userModel = userResponse.Record.Fields;
        Logger.LogInformation($"User found: {userModel.Name} ({userModel.Telegram}) - Role: {userModel.Role}");

        // Validate user permissions: must be the owner OR have Admin role
        var isOwner = catModel.OwnerRecordId == request.UserRecordId;
        var isAdmin = userModel.Role == UserRoles.Admin;

        if (!isOwner && !isAdmin)
        {
            throw new Exception($"Access denied. User '{userModel.Name}' ({userModel.Telegram}) is not the owner of cat '{catModel.Name}' and does not have Admin role. " +
                $"Only the cat owner or admins can remove a cat from an event.");
        }

        Logger.LogInformation($"Permission granted: User '{userModel.Name}' is {(isOwner ? "the owner" : "an admin")}");

        // Validate event exists and get current state
        var eventResponse = await AirtableRepository.RetrieveRecord<AtEvent>(EventTableName, request.EventRecordId);
        if (!eventResponse.Success || eventResponse.Record == null)
        {
            throw new Exception($"Event with ID {request.EventRecordId} not found.");
        }

        var eventModel = eventResponse.Record.Fields;
        Logger.LogInformation($"Event found: {eventModel.Name}");

        // Check if cat is registered for this event
        var currentCats = eventModel.Cats ?? Array.Empty<string>();
        if (!currentCats.Contains(request.CatRecordId))
        {
            throw new Exception($"Cat '{catModel.Name}' is not registered for event '{eventModel.Name}'");
        }

        // Remove cat from event
        var updatedCats = currentCats.Where(catId => catId != request.CatRecordId).ToArray();
        var updatedFields = new Fields();
        updatedFields.AddField("Cats", updatedCats);

        var updateResponse = await AirtableRepository.UpdateRecord(EventTableName, updatedFields, request.EventRecordId);

        if (!updateResponse.Success)
        {
            Logger.LogError($"Error excluding cat from event: {updateResponse.AirtableApiError.ErrorMessage}");
            throw new Exception($"Error excluding cat '{catModel.Name}' from event '{eventModel.Name}': {updateResponse.AirtableApiError.ErrorMessage}");
        }

        Logger.LogInformation($"Successfully removed cat '{catModel.Name}' from event '{eventModel.Name}' by user '{userModel.Name}'");

        // Optional: Send notification to cat owner
        if (catModel.OwnerTelegramChatId != 0)
        {
            try
            {
                // Use the owner's language for localization
                var ownerLanguage = Languages.en;
                var ownerResponse = await AirtableRepository.RetrieveRecord<AtUser>(UserTableName, catModel.OwnerRecordId);
                if (ownerResponse.Success && ownerResponse.Record != null)
                {
                    ownerLanguage = ownerResponse.Record.Fields.Language;
                }
                var message = LocalizationService.GetCatRemovedFromEventMessage(ownerLanguage, catModel.Name, eventModel.Name, eventModel.When, eventModel.Where);
                await TelegramBotClient.SendMessage(catModel.OwnerTelegramChatId, message);
                Logger.LogInformation($"Notification sent to cat owner: {catModel.OwnerName}");
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, $"Failed to send notification to cat owner {catModel.OwnerName}");
                // Don't throw here, exclusion was successful
            }
        }
        // Send admin notification
        try
        {
            await AdminService.NotifyAdminsAboutCatRemovedFromEvent(
                catModel.Name,
                catModel.OwnerName!,
                catModel.RecordId!,
                eventModel.Name,
                eventModel.RecordId!,
                userModel.Name,
                userModel.Telegram
            );
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, $"Failed to notify admins about cat removal from event for {catModel.Name}");
        }

        return true;
    }

        public async Task<bool> MarkCatAsAdopted(string catRecordId, string userRecordId, string? adoptionComment = null)
    {
        Logger.LogInformation($"Marking cat as adopted: {catRecordId} by user: {userRecordId}");

        // Validate cat exists
        var catResponse = await AirtableRepository.RetrieveRecord<AtCat>(CatTableName, catRecordId);
        if (!catResponse.Success || catResponse.Record == null)
        {
            throw new Exception($"Cat with ID {catRecordId} not found.");
        }

        var catModel = catResponse.Record.Fields;
        Logger.LogInformation($"Cat found: {catModel.Name} (Owner: {catModel.OwnerName})");

        // Validate user exists and get user details
        var userResponse = await AirtableRepository.RetrieveRecord<AtUser>(UserTableName, userRecordId);
        if (!userResponse.Success || userResponse.Record == null)
        {
            throw new Exception($"User with ID {userRecordId} not found.");
        }

        var userModel = userResponse.Record.Fields;
        Logger.LogInformation($"User found: {userModel.Name} ({userModel.Telegram}) - Role: {userModel.Role}");

        // Validate user permissions: must be the owner OR have Admin role
        var isOwner = catModel.OwnerRecordId == userRecordId;
        var isAdmin = userModel.Role == UserRoles.Admin;

        if (!isOwner && !isAdmin)
        {
            throw new Exception($"Access denied. User '{userModel.Name}' ({userModel.Telegram}) is not the owner of cat '{catModel.Name}' and does not have Admin role. " +
                $"Only the cat owner or admins can mark a cat as adopted.");
        }

        Logger.LogInformation($"Permission granted: User '{userModel.Name}' is {(isOwner ? "the owner" : "an admin")}");

        // Check if cat is already adopted
        if (catModel.Status == CatStatuses.Adopted)
        {
            throw new Exception($"Cat '{catModel.Name}' is already marked as adopted.");
        }

        // Update cat status to Adopted and add adoption comment if provided
        var updatedFields = new Fields();
        updatedFields.AddField("Status", CatStatuses.Adopted.ToString());

        if (!string.IsNullOrWhiteSpace(adoptionComment))
        {
            updatedFields.AddField("AdoptionComment", adoptionComment);
            Logger.LogInformation($"Adding adoption comment for cat '{catModel.Name}': {adoptionComment}");
        }

        var updateResponse = await AirtableRepository.UpdateRecord(CatTableName, updatedFields, catRecordId);

        if (!updateResponse.Success)
        {
            Logger.LogError($"Error marking cat as adopted: {updateResponse.AirtableApiError.ErrorMessage}");
            throw new Exception($"Error marking cat '{catModel.Name}' as adopted: {updateResponse.AirtableApiError.ErrorMessage}");
        }

        Logger.LogInformation($"Successfully marked cat '{catModel.Name}' as adopted by user '{userModel.Name}'");

        // Notify admins about the adoption, including who performed the action
        try
        {
            await AdminService.NotifyAdminsAboutCatAdoption(catModel.Name, catModel.OwnerName!, catRecordId, adoptionComment, userModel.Name, userModel.Telegram);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, $"Failed to notify admins about cat adoption for {catModel.Name}");
            // Don't throw here - status change was successful, notification failure shouldn't fail the operation
        }

        return true;
    }

    public async Task<PaginatedResponse<CatDto>> GetCatsForAdmin(GetCatsForAdminRequest request)
    {
        Logger.LogInformation($"Getting cats for admin - Status: {request.Status}, Offset: {request.Offset}, TextFilter: {request.TextFilter}, PaidFilter: {request.PaidFilter}, FreeFilter: {request.FreeFilter}, IsCatebiFilter: {request.IsCatebiFilter}");

        // Build filter formula for Airtable
        var filters = new List<string>();

        // Add status filter if provided
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            filters.Add($"{{Status}} = '{request.Status}'");
        }

        // Add text filter if provided (searches in cat name, owner name, and owner telegram)
        if (!string.IsNullOrWhiteSpace(request.TextFilter))
        {
            var textSearchFilter = $"OR(" +
                $"FIND(LOWER('{request.TextFilter.ToLower()}'), LOWER({{Name}})) > 0, " +
                $"FIND(LOWER('{request.TextFilter.ToLower()}'), LOWER(ARRAYJOIN({{OwnerName}}, ','))) > 0, " +
                $"FIND(LOWER('{request.TextFilter.ToLower()}'), LOWER(ARRAYJOIN({{OwnerTelegram}}, ','))) > 0" +
                $")";
            filters.Add(textSearchFilter);
        }

        // Add payment status filters
        if (request.PaidFilter.HasValue && request.PaidFilter.Value)
        {
            filters.Add($"{{AccountPaymentStatus}} = '{CatPaymentStatuses.Confirmed}'");
        }

        if (request.FreeFilter.HasValue && request.FreeFilter.Value)
        {
            filters.Add($"OR({{AccountPaymentStatus}} = '', {{AccountPaymentStatus}} != '{CatPaymentStatuses.Confirmed}')");
        }

        // Add IsCatebiCat filter
        if (request.IsCatebiFilter.HasValue)
        {
            filters.Add($"{{IsCatebiCat}} = {(request.IsCatebiFilter.Value ? "1" : "0")}");
        }

        // Combine filters with AND
        string filterFormula = string.Empty;
        if (filters.Any())
        {
            filterFormula = filters.Count == 1
                ? filters[0]
                : $"AND({string.Join(", ", filters)})";
        }

        try
        {
            // Configure pagination
            const int pageSize = 10;

            // Set up sorting to get consistent results
            var sort = new List<Sort>
            {
                new() { Field = "Created", Direction = SortDirection.Desc }
            };

                        // Use offset for pagination (null for first page)
            var offsetToUse = request.Offset;

            if (string.IsNullOrEmpty(offsetToUse))
            {
                Logger.LogInformation("Starting pagination from first page");
            }
            else
            {
                Logger.LogInformation($"Using provided offset for pagination: {offsetToUse}");
            }

            var response = await AirtableRepository.ListRecords<AtCat>(
                CatTableName,
                filterByFormula: filterFormula,
                pageSize: pageSize,
                offset: offsetToUse,
                sort: sort
            );

            if (!response.Success)
            {
                Logger.LogError($"Error getting cats for admin: {response.AirtableApiError.ErrorMessage}");
                throw new Exception($"Error getting cats for admin: {response.AirtableApiError.ErrorMessage}");
            }

            var cats = new List<CatDto>();

            foreach (var record in response.Records)
            {
                cats.Add(CatConverter.ToDto(record.Fields, 0, record.Id));
            }

            var totalCats = cats.Count;
            Logger.LogInformation($"Retrieved {totalCats} cats from Airtable for request (Offset: {request.Offset})");

            return new PaginatedResponse<CatDto>
            {
                Records = cats,
                NextPageOffset = response.Offset, // Use actual Airtable offset for next page
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in GetCatsForAdmin");
            throw;
        }
    }
}
