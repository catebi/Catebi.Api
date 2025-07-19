using AirtableApiClient;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Catebi.Api.Domain.Features.AdoptionBot.Enums;
using Catebi.Api.Domain.Features.AdoptionBot.Converters;
using Catebi.Api.Domain.Features.AdoptionBot.Models;
using Catebi.Api.Domain.Features.AdoptionBot.ViewModels;

namespace Catebi.Api.Domain.Features.AdoptionBot;

public class AdoptionBotCatbookService(
    IAirtableRepository AirtableRepository,
    CommonTelegramBotClient CommonTelegramBotClient,
    ISettingsService SettingsService,
    ILogger<AdoptionBotCatbookService> Logger) : IAdoptionBotCatbookService
{
    private readonly string CatbookTableName = AirTables.Catbook.ToString();
    private readonly string CatTableName = AirTables.Cat.ToString();
    private readonly string StatusColumnName = "Status";

    public async Task<CatbookInfoDto?> GetCatbookInfo(string catRecordId)
    {
        Logger.LogInformation($"Getting catbook info for cat record ID: {catRecordId}");

        // Find catbook record by cat record ID
        var response = await AirtableRepository.ListRecords<AtCatbook>(
            CatbookTableName,
            filterByFormula: $"{{CatRecordId}}='{catRecordId}'"
        );

        if (!response.Success)
        {
            Logger.LogError($"Error getting catbook info: {response.AirtableApiError.ErrorMessage}");
            throw new Exception($"Error getting catbook info: {response.AirtableApiError.ErrorMessage}");
        }

        var catbookRecord = response.Records.FirstOrDefault();
        if (catbookRecord == null)
        {
            Logger.LogInformation($"No catbook record found for cat ID: {catRecordId}");
            return null;
        }

        Logger.LogInformation($"Found catbook record for cat ID: {catRecordId}");
        return CatbookConverter.ToDto(catbookRecord.Fields, catbookRecord.Id);
    }

    public async Task<CatbookInfoDto> SaveCatbookInfo(CatbookInfoDto catbookInfo)
    {
        Logger.LogInformation($"Saving new catbook info for cat record ID: {catbookInfo.CatRecordId}");

        // Validate cat exists
        if (string.IsNullOrEmpty(catbookInfo.CatRecordId))
        {
            throw new Exception("Cat record ID is required");
        }

        var catResponse = await AirtableRepository.RetrieveRecord<AtCat>(CatTableName, catbookInfo.CatRecordId);
        if (!catResponse.Success || catResponse.Record == null)
        {
            throw new Exception($"Cat with ID {catbookInfo.CatRecordId} not found.");
        }

        Logger.LogInformation($"Cat validation passed for: {catResponse.Record.Fields.Name}");

        // Check if catbook record already exists for this cat
        var existingResponse = await AirtableRepository.ListRecords<AtCatbook>(
            CatbookTableName,
            filterByFormula: $"{{CatRecordId}}='{catbookInfo.CatRecordId}'"
        );

        if (existingResponse.Success && existingResponse.Records.Any())
        {
            throw new Exception($"Catbook record already exists for cat {catbookInfo.CatRecordId}");
        }

        var fields = new Fields();
        fields.AddField("Cat", new string[] { catbookInfo.CatRecordId });
        fields.AddField("Status", CatbookStatuses.ToConfirm.ToString());

        // Add all the catbook fields
        if (!string.IsNullOrEmpty(catbookInfo.Descr))
            fields.AddField("Descr", catbookInfo.Descr);
        if (!string.IsNullOrEmpty(catbookInfo.Color))
            fields.AddField("Color", catbookInfo.Color);
        if (!string.IsNullOrEmpty(catbookInfo.Aliases))
            fields.AddField("Aliases", catbookInfo.Aliases);
        if (!string.IsNullOrEmpty(catbookInfo.MediaLink))
            fields.AddField("MediaLink", catbookInfo.MediaLink);
        if (!string.IsNullOrEmpty(catbookInfo.HealthNotes))
            fields.AddField("HealthNotes", catbookInfo.HealthNotes);
        if (catbookInfo.HasPassport.HasValue)
            fields.AddField("HasPassport", catbookInfo.HasPassport.Value);
        if (!string.IsNullOrEmpty(catbookInfo.CharacterNotes))
            fields.AddField("CharacterNotes", catbookInfo.CharacterNotes);
        if (!string.IsNullOrEmpty(catbookInfo.HistoryNotes))
            fields.AddField("HistoryNotes", catbookInfo.HistoryNotes);
        if (!string.IsNullOrEmpty(catbookInfo.Location))
            fields.AddField("Location", catbookInfo.Location);
        if (catbookInfo.DeliveryAvailable.HasValue)
            fields.AddField("DeliveryAvailable", catbookInfo.DeliveryAvailable.Value);
        if (!string.IsNullOrEmpty(catbookInfo.DeliveryNotes))
            fields.AddField("DeliveryNotes", catbookInfo.DeliveryNotes);
        if (!string.IsNullOrEmpty(catbookInfo.ContactTg))
            fields.AddField("ContactTg", catbookInfo.ContactTg);

        var createResponse = await AirtableRepository.CreateRecord(CatbookTableName, fields);

        if (!createResponse.Success)
        {
            Logger.LogError($"Error creating catbook record: {createResponse.AirtableApiError.ErrorMessage}");
            throw new Exception($"Error creating catbook record: {createResponse.AirtableApiError.ErrorMessage}");
        }

        Logger.LogInformation($"Catbook record created successfully for cat: {catbookInfo.CatRecordId}");

        // Return the created record
        var createdRecord = await AirtableRepository.RetrieveRecord<AtCatbook>(CatbookTableName, createResponse.Record.Id);
        return CatbookConverter.ToDto(createdRecord.Record.Fields, createResponse.Record.Id);
    }

    public async Task<CatbookInfoDto> UpdateCatbookInfo(CatbookInfoDto catbookInfo)
    {
        Logger.LogInformation($"Updating catbook info for record ID: {catbookInfo.RecordId}");

        if (string.IsNullOrEmpty(catbookInfo.RecordId))
        {
            throw new Exception("Record ID is required for updating catbook info");
        }

        // Validate catbook record exists
        var existingResponse = await AirtableRepository.RetrieveRecord<AtCatbook>(CatbookTableName, catbookInfo.RecordId);
        if (!existingResponse.Success || existingResponse.Record == null)
        {
            throw new Exception($"Catbook record with ID {catbookInfo.RecordId} not found.");
        }

        Logger.LogInformation($"Catbook record validation passed for ID: {catbookInfo.RecordId}");

        var fields = new Fields();

        // Update all fields except status (as mentioned in requirements)
        if (!string.IsNullOrEmpty(catbookInfo.Descr))
            fields.AddField("Descr", catbookInfo.Descr);
        if (!string.IsNullOrEmpty(catbookInfo.Color))
            fields.AddField("Color", catbookInfo.Color);
        if (!string.IsNullOrEmpty(catbookInfo.Aliases))
            fields.AddField("Aliases", catbookInfo.Aliases);
        if (!string.IsNullOrEmpty(catbookInfo.MediaLink))
            fields.AddField("MediaLink", catbookInfo.MediaLink);
        if (!string.IsNullOrEmpty(catbookInfo.HealthNotes))
            fields.AddField("HealthNotes", catbookInfo.HealthNotes);
        if (catbookInfo.HasPassport.HasValue)
            fields.AddField("HasPassport", catbookInfo.HasPassport.Value);
        if (!string.IsNullOrEmpty(catbookInfo.CharacterNotes))
            fields.AddField("CharacterNotes", catbookInfo.CharacterNotes);
        if (!string.IsNullOrEmpty(catbookInfo.HistoryNotes))
            fields.AddField("HistoryNotes", catbookInfo.HistoryNotes);
        if (!string.IsNullOrEmpty(catbookInfo.Location))
            fields.AddField("Location", catbookInfo.Location);
        if (catbookInfo.DeliveryAvailable.HasValue)
            fields.AddField("DeliveryAvailable", catbookInfo.DeliveryAvailable.Value);
        if (!string.IsNullOrEmpty(catbookInfo.DeliveryNotes))
            fields.AddField("DeliveryNotes", catbookInfo.DeliveryNotes);
        if (!string.IsNullOrEmpty(catbookInfo.ContactTg))
            fields.AddField("ContactTg", catbookInfo.ContactTg);
        if (!string.IsNullOrEmpty(catbookInfo.CatbookLink))
            fields.AddField("CatbookLink", catbookInfo.CatbookLink);
        if (catbookInfo.CatbookPostId.HasValue)
            fields.AddField("CatbookPostId", catbookInfo.CatbookPostId.Value);

        var updateResponse = await AirtableRepository.UpdateRecord(CatbookTableName, fields, catbookInfo.RecordId);

        if (!updateResponse.Success)
        {
            Logger.LogError($"Error updating catbook record: {updateResponse.AirtableApiError.ErrorMessage}");
            throw new Exception($"Error updating catbook record: {updateResponse.AirtableApiError.ErrorMessage}");
        }

        Logger.LogInformation($"Catbook record updated successfully for ID: {catbookInfo.RecordId}");

        // Return the updated record
        var updatedRecord = await AirtableRepository.RetrieveRecord<AtCatbook>(CatbookTableName, catbookInfo.RecordId);
        return CatbookConverter.ToDto(updatedRecord.Record.Fields, catbookInfo.RecordId);
    }

    public async Task<bool> ConfirmCatbookInfo(string catRecordId)
    {
        Logger.LogInformation($"Confirming catbook info for cat record ID: {catRecordId}");

        // Find catbook record by cat record ID
        var response = await AirtableRepository.ListRecords<AtCatbook>(
            CatbookTableName,
            filterByFormula: $"{{CatRecordId}}='{catRecordId}'"
        );

        if (!response.Success)
        {
            Logger.LogError($"Error finding catbook record: {response.AirtableApiError.ErrorMessage}");
            throw new Exception($"Error finding catbook record: {response.AirtableApiError.ErrorMessage}");
        }

        var catbookRecord = response.Records.FirstOrDefault();
        if (catbookRecord == null)
        {
            throw new Exception($"No catbook record found for cat ID: {catRecordId}");
        }

        if (catbookRecord.Fields.Status != CatbookStatuses.ToConfirm)
        {
            throw new Exception($"Catbook record for cat {catRecordId} is not in ToConfirm status");
        }

        // Get cat information for the telegram post
        var catResponse = await AirtableRepository.RetrieveRecord<AtCat>(CatTableName, catRecordId);
        if (!catResponse.Success || catResponse.Record == null)
        {
            throw new Exception($"Cat with ID {catRecordId} not found.");
        }

        var cat = catResponse.Record.Fields;
        Logger.LogInformation($"Cat information retrieved: {cat.Name}");

        // Update status to Confirmed
        var fields = new Fields();
        fields.AddField(StatusColumnName, CatbookStatuses.Confirmed.ToString());

        var updateResponse = await AirtableRepository.UpdateRecord(CatbookTableName, fields, catbookRecord.Id);

        if (!updateResponse.Success)
        {
            Logger.LogError($"Error confirming catbook record: {updateResponse.AirtableApiError.ErrorMessage}");
            throw new Exception($"Error confirming catbook record: {updateResponse.AirtableApiError.ErrorMessage}");
        }

        Logger.LogInformation($"Catbook record confirmed successfully for cat: {cat.Name}");

        // Send post to catbook telegram channel
        try
        {
            await SendCatbookPostToTelegram(catbookRecord.Fields, cat);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, $"Failed to send telegram post for catbook {catbookRecord.Id}");
            // Don't throw here - confirmation was successful, telegram failure shouldn't fail the operation
        }

        return true;
    }

    public async Task<bool> ArchiveCatbookInfo(string catRecordId)
    {
        Logger.LogInformation($"Archiving catbook info for cat record ID: {catRecordId}");

        // Find catbook record by cat record ID
        var response = await AirtableRepository.ListRecords<AtCatbook>(
            CatbookTableName,
            filterByFormula: $"{{CatRecordId}}='{catRecordId}'"
        );

        if (!response.Success)
        {
            Logger.LogError($"Error finding catbook record: {response.AirtableApiError.ErrorMessage}");
            throw new Exception($"Error finding catbook record: {response.AirtableApiError.ErrorMessage}");
        }

        var catbookRecord = response.Records.FirstOrDefault();
        if (catbookRecord == null)
        {
            throw new Exception($"No catbook record found for cat ID: {catRecordId}");
        }

        // Update status to Archived
        var fields = new Fields();
        fields.AddField(StatusColumnName, CatbookStatuses.Archived.ToString());

        var updateResponse = await AirtableRepository.UpdateRecord(CatbookTableName, fields, catbookRecord.Id);

        if (!updateResponse.Success)
        {
            Logger.LogError($"Error archiving catbook record: {updateResponse.AirtableApiError.ErrorMessage}");
            throw new Exception($"Error archiving catbook record: {updateResponse.AirtableApiError.ErrorMessage}");
        }

        Logger.LogInformation($"Catbook record archived successfully for cat ID: {catRecordId}");
        return true;
    }

    public async Task<IEnumerable<CatbookInfoDto>> GetCatbookInfosToConfirm()
    {
        Logger.LogInformation("Getting all catbook records with ToConfirm status");

        var response = await AirtableRepository.ListRecords<AtCatbook>(
            CatbookTableName,
            filterByFormula: $"{{Status}}='{CatbookStatuses.ToConfirm}'"
        );

        if (!response.Success)
        {
            Logger.LogError($"Error getting catbook records to confirm: {response.AirtableApiError.ErrorMessage}");
            throw new Exception($"Error getting catbook records to confirm: {response.AirtableApiError.ErrorMessage}");
        }

        var catbookInfos = response.Records.Select(r => CatbookConverter.ToDto(r.Fields, r.Id)).ToList();
        Logger.LogInformation($"Found {catbookInfos.Count} catbook records to confirm");

        return catbookInfos;
    }

    private async Task SendCatbookPostToTelegram(AtCatbook catbook, AtCat cat)
    {
        Logger.LogInformation($"Sending catbook post to Telegram for cat: {cat.Name}");

        var (chatId, topicId) = await SettingsService.GetChatTopicInfo();

        // Build the catbook message
        var message = BuildCatbookMessage(catbook, cat);

        try
        {
            Telegram.Bot.Types.Message sentMessage;

            // Collect all photos (main photo + additional photos)
            var allPhotos = new List<string>();

            if (cat.MainPhoto != null && cat.MainPhoto.Length > 0)
            {
                allPhotos.AddRange(cat.MainPhoto.Select(p => p.Url));
            }

            if (cat.Photos != null && cat.Photos.Length > 0)
            {
                allPhotos.AddRange(cat.Photos.Select(p => p.Url));
            }

            if (allPhotos.Count > 0)
            {
                // Send photos with the message as caption on the first photo
                if (allPhotos.Count == 1)
                {
                    // Single photo
                    sentMessage = await CommonTelegramBotClient.Client.SendMessage(
                        chatId: chatId,
                        text: message,
                        // photo: InputFile.FromUri(allPhotos[0]),
                        // caption: message,
                        parseMode: ParseMode.Html,
                        messageThreadId: (int)topicId
                    );
                    // sentMessage = await CommonTelegramBotClient.Client.SendPhoto(
                    //     chatId: chatId,
                    //     photo: InputFile.FromUri(allPhotos[0]),
                    //     caption: message,
                    //     parseMode: ParseMode.Html,
                    //     messageThreadId: (int)topicId
                    // );
                }
                else
                {
                    // Multiple photos - create media group
                    var mediaGroup = new List<IAlbumInputMedia>();

                    for (var i = 0; i < allPhotos.Count && i < 10; i++) // Telegram limit is 10 media items
                    {
                        if (i == 0)
                        {
                            // First photo gets the caption
                            mediaGroup.Add(new InputMediaPhoto(InputFile.FromUri(allPhotos[i]))
                            {
                                Caption = message,
                                ParseMode = ParseMode.Html
                            });
                        }
                        else
                        {
                            mediaGroup.Add(new InputMediaPhoto(InputFile.FromUri(allPhotos[i])));
                        }
                    }

                    var mediaMessages = await CommonTelegramBotClient.Client.SendMediaGroup(
                        chatId: chatId,
                        media: mediaGroup,
                        messageThreadId: (int)topicId
                    );

                    sentMessage = mediaMessages.First(); // Use first message for ID
                }
            }
            else
            {
                // No photos, send text only
                sentMessage = await CommonTelegramBotClient.Client.SendMessage(
                    chatId: chatId,
                    text: message,
                    parseMode: ParseMode.Html,
                    messageThreadId: (int)topicId
                );
            }

            // Update the catbook record with the post ID and potentially link
            var fields = new Fields();
            fields.AddField("CatbookPostId", sentMessage.MessageId);

            // If there's a way to generate a link to the post, add it here
            // For now, we'll just store the message ID

            await AirtableRepository.UpdateRecord(CatbookTableName, fields, catbook.RecordId!);

            Logger.LogInformation($"Catbook post sent successfully for cat: {cat.Name}, Message ID: {sentMessage.MessageId}");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Error sending catbook post to Telegram for cat: {cat.Name}");
            throw;
        }
    }

    private static string BuildCatbookMessage(AtCatbook catbook, AtCat cat)
    {
        var message = "";

        // Cat name and aliases
        if (!string.IsNullOrEmpty(catbook.Aliases))
        {
            message += $"<b>{cat.Name}, {catbook.Aliases}</b>\n";
        }
        else
        {
            message += $"<b>{cat.Name}</b>\n";
        }

        // Age calculation
        var age = CalculateAge(cat.DateOfBirth);
        if (!string.IsNullOrEmpty(age))
        {
            message += $"▪️Возраст: {age}\n";
        }

        // Sex/Gender
        var gender = GetGenderText(cat.SexValue);
        if (!string.IsNullOrEmpty(gender))
        {
            message += $"▪️Пол: {gender}\n";
        }

        // Color
        if (!string.IsNullOrEmpty(catbook.Color))
        {
            message += $"▪️Окрас: {catbook.Color}\n";
        }

        message += "\n"; // Empty line before detailed info

        // Health information
        if (!string.IsNullOrEmpty(catbook.HealthNotes))
        {
            message += $"▪️Здоровье: {catbook.HealthNotes}\n\n";
        }

        // Character/personality
        if (!string.IsNullOrEmpty(catbook.CharacterNotes))
        {
            message += $"▪️Характер: {catbook.CharacterNotes}\n\n";
        }

        // History/backstory
        if (!string.IsNullOrEmpty(catbook.HistoryNotes))
        {
            message += $"▪️Историческая справка: {catbook.HistoryNotes}\n\n";
        }

        // Location and delivery
        if (!string.IsNullOrEmpty(catbook.Location))
        {
            var locationText = catbook.Location;
            if (catbook.DeliveryAvailable.HasValue && catbook.DeliveryAvailable.Value)
            {
                if (!string.IsNullOrEmpty(catbook.DeliveryNotes))
                {
                    locationText += $", {catbook.DeliveryNotes}";
                }
                else
                {
                    locationText += ", доставка возможна";
                }
            }
            message += $"▪️Местоположение: {locationText}\n";
        }

        // Contact information
        if (!string.IsNullOrEmpty(catbook.ContactTg))
        {
            var contact = catbook.ContactTg.TrimStart('@');
            message += $"▪️Контакт для связи: @{contact}";
        }

        // Additional media link if provided
        if (!string.IsNullOrEmpty(catbook.MediaLink))
        {
            message += $"\n▪️Дополнительные медиа: {catbook.MediaLink}";
        }

        return message;
    }

    private static string CalculateAge(DateTime dateOfBirth)
    {
        var now = DateTime.Now;
        var totalMonths = ((now.Year - dateOfBirth.Year) * 12) + now.Month - dateOfBirth.Month;

        if (now.Day < dateOfBirth.Day)
        {
            totalMonths--;
        }

        if (totalMonths < 1)
        {
            var days = (now - dateOfBirth).Days;
            if (days < 7)
            {
                return $"{days} {GetDayWord(days)}";
            }
            else
            {
                var weeks = days / 7;
                return $"{weeks} {GetWeekWord(weeks)}";
            }
        }
        else if (totalMonths < 12)
        {
            return $"{totalMonths} {GetMonthWord(totalMonths)}";
        }
        else
        {
            var years = totalMonths / 12;
            var remainingMonths = totalMonths % 12;

            if (remainingMonths == 0)
            {
                return $"{years} {GetYearWord(years)}";
            }
            else
            {
                return $"{years} {GetYearWord(years)} {remainingMonths} {GetMonthWord(remainingMonths)}";
            }
        }
    }

    private static string GetGenderText(string sex) => sex.ToLower() switch
    {
        "male" => "Мальчик",
        "female" => "Девочка",
        _ => ""
    };

    private static string GetDayWord(int days) => days switch
    {
        1 => "день",
        2 or 3 or 4 => "дня",
        _ => "дней"
    };

    private static string GetWeekWord(int weeks) => weeks switch
    {
        1 => "неделя",
        2 or 3 or 4 => "недели",
        _ => "недель"
    };

    private static string GetMonthWord(int months) => months switch
    {
        1 => "месяц",
        2 or 3 or 4 => "месяца",
        _ => "месяцев"
    };

    private static string GetYearWord(int years) => years switch
    {
        1 => "год",
        2 or 3 or 4 => "года",
        _ => "лет"
    };
}
