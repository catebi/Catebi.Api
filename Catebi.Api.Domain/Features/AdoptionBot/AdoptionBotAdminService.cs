using AirtableApiClient;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Catebi.Api.Domain.Features.AdoptionBot.Enums;
using Catebi.Api.Domain.Features.AdoptionBot.Converters;

namespace Catebi.Api.Domain.Features.AdoptionBot;

public class AdoptionBotAdminService(
    IAirtableRepository AirtableRepository,
    TelegramBotClient TelegramBotClient,
    CommonTelegramBotClient CommonTelegramBotClient,
    ISettingsService SettingsService,
    ILocalizationService LocalizationService,
    ILogger<AdoptionBotAdminService> Logger) : IAdoptionBotAdminService
{
    private readonly string UserTableName = AirTables.User.ToString();
    private readonly string CatTableName = AirTables.Cat.ToString();
    private readonly string EventTableName = AirTables.Event.ToString();
    private readonly string CatPaymentName = AirTables.CatPayment.ToString();
    private readonly string MessageTableName = AirTables.Message.ToString();
    private readonly string CatbookTableName = AirTables.Catbook.ToString();
    private readonly string StatusColumnName = "Status";

    private static string ConvertHtmlToTelegramFormat(string htmlContent)
    {
        if (string.IsNullOrEmpty(htmlContent))
            return htmlContent;

        var result = htmlContent;

        // Replace HTML entities
        result = result.Replace("&nbsp;", " ");
        result = result.Replace("&#39;", "'");
        result = result.Replace("&quot;", "\"");
        result = result.Replace("&amp;", "&");
        result = result.Replace("&lt;", "<");
        result = result.Replace("&gt;", ">");

        // Handle ordered lists (convert to numbered list)
        result = System.Text.RegularExpressions.Regex.Replace(result, @"<ol[^>]*>", "");
        result = System.Text.RegularExpressions.Regex.Replace(result, @"</ol>", "\n");

        // Handle unordered lists (convert to bullet list)
        result = System.Text.RegularExpressions.Regex.Replace(result, @"<ul[^>]*>", "");
        result = System.Text.RegularExpressions.Regex.Replace(result, @"</ul>", "\n");

        // Handle list items - this is more complex as we need to track if we're in ol or ul
        // For simplicity, we'll use bullets for all list items
        result = System.Text.RegularExpressions.Regex.Replace(result, @"<li[^>]*>", "• ");
        result = System.Text.RegularExpressions.Regex.Replace(result, @"</li>", "\n");

        // Clean up link tags - remove unsupported attributes, keep only href
        result = System.Text.RegularExpressions.Regex.Replace(result, @"<a\s+[^>]*href\s*=\s*[""']([^""']*)[""'][^>]*>", @"<a href=""$1"">");

        // Replace paragraph tags with line breaks
        result = System.Text.RegularExpressions.Regex.Replace(result, @"<p[^>]*>", "");
        result = System.Text.RegularExpressions.Regex.Replace(result, @"</p>", "\n");

        // Replace other unsupported block elements with line breaks
        result = System.Text.RegularExpressions.Regex.Replace(result, @"<div[^>]*>", "");
        result = System.Text.RegularExpressions.Regex.Replace(result, @"</div>", "\n");
        result = System.Text.RegularExpressions.Regex.Replace(result, @"<br[^>]*>", "\n");

        // Clean up multiple consecutive line breaks
        result = System.Text.RegularExpressions.Regex.Replace(result, @"\n\s*\n", "\n\n");

        // Trim whitespace
        result = result.Trim();

        return result;
    }

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

        var messageRecordId = createResponse.Record.Id;

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

        // Send message to all confirmed users and collect log information
        var successCount = 0;
        var failCount = 0;
        var successfulTelegramAccounts = new List<string>();
        var errors = new List<string>();

        foreach (var user in confirmedUsers)
        {
            try
            {
                if (user.TelegramChatId != 0)
                {
                    // For broadcast messages, use the original content (admin can write in any language)
                    var localizedContent = LocalizationService.GetBroadcastMessage(user.Language, content);
                    // Convert HTML to Telegram-compatible format
                    var telegramContent = ConvertHtmlToTelegramFormat(localizedContent);
                    await TelegramBotClient.SendMessage(user.TelegramChatId, telegramContent, parseMode: ParseMode.Html);
                    successCount++;

                    // Collect successful telegram account (use telegram username or name if available)
                    var telegramAccount = !string.IsNullOrEmpty(user.Telegram) ? user.Telegram : user.Name;
                    if (!string.IsNullOrEmpty(telegramAccount))
                    {
                        successfulTelegramAccounts.Add(telegramAccount);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, $"Failed to send message to user {user.Name} (ID: {user.RecordId})");
                failCount++;

                // Collect error information
                var userIdentifier = !string.IsNullOrEmpty(user.Telegram) ? user.Telegram : user.Name;
                errors.Add($"User: {userIdentifier} - Error: {ex.Message}");
            }
        }

        Logger.LogInformation($"Broadcast completed: {successCount} success, {failCount} failed");

        // Build log information
        var logEntries = new List<string>();

        // Add success and fail counts
        logEntries.Add($"Success: {successCount}, Failed: {failCount}");

        // Add successful telegram accounts (joined by commas)
        if (successfulTelegramAccounts.Any())
        {
            logEntries.Add($"Successful recipients: {string.Join(", ", successfulTelegramAccounts)}");
        }

        // Add errors if any occurred
        if (errors.Any())
        {
            logEntries.Add("Errors:");
            logEntries.AddRange(errors);
        }

        var logContent = string.Join("\n", logEntries);

        // Determine message status based on failures
        var messageStatus = failCount > 0 ? MessageStatuses.FailedToSend : MessageStatuses.SuccessfullySent;

        // Update the message record with log information and status
        var updateFields = new Fields();
        updateFields.AddField("Log", logContent);
        updateFields.AddField("Status", messageStatus.ToString());
        var updateResponse = await AirtableRepository.UpdateRecord(MessageTableName, updateFields, messageRecordId);

        if (!updateResponse.Success)
        {
            Logger.LogWarning($"Failed to update message log: {updateResponse.AirtableApiError.ErrorMessage}");
        }

        // Retrieve the updated message to return
        var messageRecord = await AirtableRepository.RetrieveRecord<AtMessage>(MessageTableName, messageRecordId);
        if (!messageRecord.Success || messageRecord.Record == null)
        {
            throw new Exception($"Error retrieving created message: {createResponse.AirtableApiError.ErrorMessage}");
        }

        return MessageConverter.ToDto(messageRecord.Record.Fields);
    }

    public async Task<IEnumerable<MessageDto>> GetBroadcastMessages()
    {
        Logger.LogInformation("Getting all broadcast messages");

        var response = await AirtableRepository.ListRecords<AtMessage>(MessageTableName, filterByFormula: null);

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

    public async Task<bool> NotifyAdminsAboutUserRegistration(string userName, string userTelegram, string userRecordId)
    {
        Logger.LogInformation($"Notifying work chat about new user registration: {userName}");

        try
        {
            var (workChatId, eventTopicId) = await SettingsService.GetChatTopicInfo();
            var message = LocalizationService.GetAdminUserRegistrationNotification(
                Languages.ru, userName, userTelegram, userRecordId);

            var keyboard = new InlineKeyboardMarkup(
            [
                [
                    InlineKeyboardButton.WithUrl("👥 Open Admin Users", "t.me/CatebiAdoptionBot/eventappa?startapp=admin_users")
                ]
            ]);

            await CommonTelegramBotClient.Client.SendMessage(
                chatId: workChatId,
                message,
                parseMode: ParseMode.Html,
                replyMarkup: keyboard,
                messageThreadId: (int)eventTopicId
            );

            Logger.LogInformation($"Work chat notification sent successfully for user registration: {userName}");
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Error sending work chat notification for user registration: {userName}");
            return false;
        }
    }

    public async Task<bool> NotifyAdminsAboutPaymentSubmission(string catName, string ownerName, string catRecordId, string paymentRecordId)
    {
        Logger.LogInformation($"Notifying work chat about new payment submission for cat: {catName}");

        try
        {
            var (workChatId, eventTopicId) = await SettingsService.GetChatTopicInfo();
            var message = LocalizationService.GetAdminPaymentSubmissionNotification(
                Languages.ru, catName, ownerName, catRecordId, paymentRecordId);
            var keyboard = new InlineKeyboardMarkup(
            [
                [
                    InlineKeyboardButton.WithUrl("💳 Open Admin Payments", "t.me/CatebiAdoptionBot/eventappa?startapp=admin_payments")
                ]
            ]);

            await CommonTelegramBotClient.Client.SendMessage(
                chatId: workChatId,
                message,
                parseMode: ParseMode.Html,
                replyMarkup: keyboard,
                messageThreadId: (int)eventTopicId
            );

            Logger.LogInformation($"Work chat notification sent successfully for payment submission: {catName}");
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Error sending work chat notification for payment submission: {catName}");
            return false;
        }
    }

    public async Task<bool> NotifyAdminsAboutCatAdoption(string catName, string ownerName, string catRecordId, string? adoptionComment = null, string? actionByUserName = null, string? actionByUserTelegram = null)
    {
        Logger.LogInformation($"Notifying work chat about cat adoption: {catName} (action by: {actionByUserName} {actionByUserTelegram})");

        try
        {
            var (workChatId, eventTopicId) = await SettingsService.GetChatTopicInfo();

            // Use Russian language for work chat notifications (can be made configurable)
            var message = LocalizationService.GetAdminCatAdoptionNotification(Languages.ru, catName, ownerName, adoptionComment, actionByUserName, actionByUserTelegram);

            // Create inline keyboard with direct mini app link to specific cat profile
            var keyboard = new InlineKeyboardMarkup(
            [
                [
                    InlineKeyboardButton.WithUrl("🐱 View Cat Profile", $"t.me/CatebiAdoptionBot/eventappa?startapp=admin_cat_{catRecordId}")
                ]
            ]);

            await CommonTelegramBotClient.Client.SendMessage(
                chatId: workChatId,
                message,
                parseMode: ParseMode.Html,
                replyMarkup: keyboard,
                messageThreadId: (int)eventTopicId
            );

            Logger.LogInformation($"Work chat notification sent successfully for cat adoption: {catName}");
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Error sending work chat notification for cat adoption: {catName}");
            return false;
        }
    }

    public async Task<bool> NotifyAdminsAboutCatRegisteredToEvent(string catName, string ownerName, string catRecordId, string eventName, string eventRecordId, string? actionByUserName = null, string? actionByUserTelegram = null)
    {
        Logger.LogInformation($"Notifying work chat about cat registered to event: {catName} (event: {eventName}, action by: {actionByUserName} {actionByUserTelegram})");
        try
        {
            var (workChatId, eventTopicId) = await SettingsService.GetChatTopicInfo();
            var message = LocalizationService.GetAdminCatRegisteredToEventNotification(
                Languages.ru, catName, ownerName, catRecordId, eventName, eventRecordId, actionByUserName, actionByUserTelegram);
            var keyboard = new InlineKeyboardMarkup(
            [
                [
                    InlineKeyboardButton.WithUrl("🐱 View Cat Profile", $"t.me/CatebiAdoptionBot/eventappa?startapp=admin_cat_{catRecordId}")
                ]
            ]);
            await CommonTelegramBotClient.Client.SendMessage(
                chatId: workChatId,
                message,
                parseMode: ParseMode.Html,
                replyMarkup: keyboard,
                messageThreadId: (int)eventTopicId
            );
            Logger.LogInformation($"Work chat notification sent successfully for cat registered to event: {catName}");
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Error sending work chat notification for cat registered to event: {catName}");
            return false;
        }
    }

    public async Task<bool> NotifyAdminsAboutCatRemovedFromEvent(string catName, string ownerName, string catRecordId, string eventName, string eventRecordId, string? actionByUserName = null, string? actionByUserTelegram = null)
    {
        Logger.LogInformation($"Notifying work chat about cat removed from event: {catName} (event: {eventName}, action by: {actionByUserName} {actionByUserTelegram})");
        try
        {
            var (workChatId, eventTopicId) = await SettingsService.GetChatTopicInfo();
            var message = LocalizationService.GetAdminCatRemovedFromEventNotification(
                Languages.ru, catName, ownerName, catRecordId, eventName, eventRecordId, actionByUserName, actionByUserTelegram);
            var keyboard = new InlineKeyboardMarkup(
            [
                [
                    InlineKeyboardButton.WithUrl("🐱 View Cat Profile", $"t.me/CatebiAdoptionBot/eventappa?startapp=admin_cat_{catRecordId}")
                ]
            ]);
            await CommonTelegramBotClient.Client.SendMessage(
                chatId: workChatId,
                message,
                parseMode: ParseMode.Html,
                replyMarkup: keyboard,
                messageThreadId: (int)eventTopicId
            );
            Logger.LogInformation($"Work chat notification sent successfully for cat removed from event: {catName}");
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Error sending work chat notification for cat removed from event: {catName}");
            return false;
        }
    }

    public async Task<IEnumerable<AtUser>> GetAdminUsers()
    {
        Logger.LogInformation("Getting all admin users");

        var adminsResponse = await AirtableRepository.ListRecords<AtUser>(
            UserTableName,
            filterByFormula: $"AND({{Role}} = '{UserRoles.Admin}')"
        );

        if (!adminsResponse.Success)
        {
            Logger.LogWarning($"Error getting admin users: {adminsResponse.AirtableApiError.ErrorMessage}");
            throw new Exception($"Error getting admin users: {adminsResponse.AirtableApiError.ErrorMessage}");
        }

        var adminUsers = adminsResponse.Records.Select(r => r.Fields).ToList();
        Logger.LogInformation($"Found {adminUsers.Count} admin users");

        return adminUsers;
    }

    public async Task<DashboardInfoDto> GetDashboardInfo()
    {
        var result = new DashboardInfoDto();

        var users    = await AirtableRepository.ListRecordsAutoFields<AtUserShort>(UserTableName);
        var cats     = await AirtableRepository.ListRecordsAutoFields<AtCatShort>(CatTableName);
        var payments = await AirtableRepository.ListRecordsAutoFields<AtCatPaymentShort>(CatPaymentName);
        var events   = await AirtableRepository.ListRecordsAutoFields<AtEventShort>(EventTableName);
        var catbooks = await AirtableRepository.ListRecordsAutoFields<AtCatbookShort>(CatbookTableName);

        result.Items.Add(new DashboardInfoItemDto
        {
            Type = DashboardItemType.Users.ToString(),
            Count = users.Count(),
            ToConfirmCount = users.Count(u => u.Status == UserStatuses.ToConfirm)
        });

        result.Items.Add(new DashboardInfoItemDto
        {
            Type = DashboardItemType.Payments.ToString(),
            Count = payments.Count(),
            ToConfirmCount = payments.Count(p => p.Status == CatPaymentStatuses.ToConfirm)
        });

        result.Items.Add(new DashboardInfoItemDto
        {
            Type = DashboardItemType.Catbook.ToString(),
            Count = catbooks.Count(),
            ToConfirmCount = catbooks.Count(c => c.Status == CatbookStatuses.ToConfirm)
        });

        result.Items.Add(new DashboardInfoItemDto
        {
            Type = DashboardItemType.Cats.ToString(),
            Count = cats.Count()
        });

        result.Items.Add(new DashboardInfoItemDto
        {
            Type = DashboardItemType.Events.ToString(),
            Count = events.Count()
        });

        return result;
    }
}
