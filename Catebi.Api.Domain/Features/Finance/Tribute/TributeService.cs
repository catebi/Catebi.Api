using AirtableApiClient;
using Catebi.Api.Domain.Features.AdoptionBot;
using Catebi.Api.Domain.Features.Finance.Tribute.Converters;
using Catebi.Api.Domain.Features.Finance.Tribute.Enums;
using Catebi.Api.Domain.Features.Finance.Tribute.Models;
using Catebi.Api.Domain.Features.Finance.Tribute.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Catebi.Api.Domain.Features.Finance.Tribute;

public class TributeService(
    [FromKeyedServices("Finance")] AirtableBase airtableBase,
    CommonTelegramBotClient telegramBotClient,
    IConfiguration configuration,
    ILogger<TributeService> logger,
    ILogger<AirtableRepository> airtableLogger) : ITributeService
{
    private readonly AirtableRepository _airtableRepository = new(airtableBase, airtableLogger);
    private readonly CommonTelegramBotClient _telegramBotClient = telegramBotClient;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<TributeService> _logger = logger;
    private readonly string _subscriptionTableName = TributeAirTables.TributeSubscription.ToString();
    private readonly string _donationTableName = TributeAirTables.TributeDonation.ToString();

    public async Task<SubscriptionDto> ProcessNewSubscription(string webhookName, NewSubscriptionPayload payload, DateTime createdAt, DateTime sentAt)
    {
        _logger.LogInformation($"Processing webhook '{webhookName}': {payload.SubscriptionName} for user {payload.TelegramUserId}");

        // Get Telegram username
        var telegramUsername = await GetTelegramUsername(payload.TelegramUserId);

        // Create Airtable record
        var fields = new Fields();
        fields.AddField("WebhookName", webhookName);
        fields.AddField("Type", webhookName);
        fields.AddField("SubscriptionName", payload.SubscriptionName);
        fields.AddField("SubscriptionId", payload.SubscriptionId);
        fields.AddField("PeriodId", payload.PeriodId);
        fields.AddField("Period", payload.Period);
        fields.AddField("Price", payload.Price);
        fields.AddField("Amount", payload.Amount);
        fields.AddField("Currency", payload.Currency);
        fields.AddField("UserId", payload.UserId);
        fields.AddField("TelegramUserId", payload.TelegramUserId.ToString());
        if (!string.IsNullOrEmpty(telegramUsername))
        {
            fields.AddField("TelegramUsername", telegramUsername);
        }
        fields.AddField("ChannelId", payload.ChannelId);
        fields.AddField("ChannelName", payload.ChannelName);
        fields.AddField("ExpiresAt", payload.ExpiresAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
        fields.AddField("CreatedAt", createdAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
        fields.AddField("SentAt", sentAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));

        var response = await _airtableRepository.CreateRecord(_subscriptionTableName, fields);

        if (!response.Success)
        {
            _logger.LogError($"Error creating subscription record: {response.AirtableApiError.ErrorMessage}");
            throw new Exception($"Error creating subscription record: {response.AirtableApiError.ErrorMessage}");
        }

        _logger.LogInformation($"Subscription record created with ID: {response.Record.Id}");

        // Send Telegram notification
        await SendSubscriptionNotification(payload, response.Record.Id);

        // Return DTO
        var subscriptionDto = new SubscriptionDto
        {
            RecordId = response.Record.Id,
            WebhookName = webhookName,
            SubscriptionName = payload.SubscriptionName,
            SubscriptionId = payload.SubscriptionId,
            PeriodId = payload.PeriodId,
            Period = payload.Period,
            Price = payload.Price,
            Amount = payload.Amount,
            Currency = payload.Currency,
            UserId = payload.UserId,
            TelegramUserId = payload.TelegramUserId,
            TelegramUsername = telegramUsername,
            ChannelId = payload.ChannelId,
            ChannelName = payload.ChannelName,
            ExpiresAt = payload.ExpiresAt,
            CreatedAt = createdAt,
            SentAt = sentAt
        };

        return subscriptionDto;
    }

    public async Task<DonationDto> ProcessNewDonation(string webhookName, NewDonationPayload payload, DateTime createdAt, DateTime sentAt)
    {
        _logger.LogInformation($"Processing webhook '{webhookName}': {payload.DonationName} for user {payload.TelegramUserId}");

        // Get Telegram username
        var telegramUsername = await GetTelegramUsername(payload.TelegramUserId);

        // Create Airtable record
        var fields = new Fields();
        fields.AddField("WebhookName", webhookName);
        fields.AddField("Type", webhookName);
        fields.AddField("DonationRequestId", payload.DonationRequestId);
        fields.AddField("DonationName", payload.DonationName);
        fields.AddField("Period", payload.Period);
        fields.AddField("Amount", payload.Amount);
        fields.AddField("Currency", payload.Currency);
        fields.AddField("Anonymously", payload.Anonymously);
        fields.AddField("WebAppLink", payload.WebAppLink);
        fields.AddField("UserId", payload.UserId);
        fields.AddField("TelegramUserId", payload.TelegramUserId.ToString());
        if (!string.IsNullOrEmpty(telegramUsername))
        {
            fields.AddField("TelegramUsername", telegramUsername);
        }
        fields.AddField("CreatedAt", createdAt);
        fields.AddField("SentAt", sentAt);

        var response = await _airtableRepository.CreateRecord(_donationTableName, fields);

        if (!response.Success)
        {
            _logger.LogError($"Error creating donation record: {response.AirtableApiError.ErrorMessage}");
            throw new Exception($"Error creating donation record: {response.AirtableApiError.ErrorMessage}");
        }

        _logger.LogInformation($"Donation record created with ID: {response.Record.Id}");

        // Send Telegram notification
        await SendNewDonationNotification(payload, response.Record.Id);

        // Return DTO
        var donationDto = new DonationDto
        {
            RecordId = response.Record.Id,
            WebhookName = webhookName,
            DonationRequestId = payload.DonationRequestId,
            DonationName = payload.DonationName,
            Period = payload.Period,
            Amount = payload.Amount,
            Currency = payload.Currency,
            Anonymously = payload.Anonymously,
            WebAppLink = payload.WebAppLink,
            UserId = payload.UserId,
            TelegramUserId = payload.TelegramUserId,
            TelegramUsername = telegramUsername,
            CreatedAt = createdAt,
            SentAt = sentAt
        };

        return donationDto;
    }

    public async Task<DonationDto> ProcessRecurrentDonation(string webhookName, RecurrentDonationPayload payload, DateTime createdAt, DateTime sentAt)
    {
        _logger.LogInformation($"Processing webhook '{webhookName}': {payload.DonationName} for user {payload.TelegramUserId}");

        // Get Telegram username
        var telegramUsername = await GetTelegramUsername(payload.TelegramUserId);

        // Create Airtable record
        var fields = new Fields();
        fields.AddField("WebhookName", webhookName);
        fields.AddField("Type", webhookName);
        fields.AddField("DonationRequestId", payload.DonationRequestId);
        fields.AddField("DonationName", payload.DonationName);
        fields.AddField("Period", payload.Period);
        fields.AddField("Amount", payload.Amount);
        fields.AddField("Currency", payload.Currency);
        fields.AddField("Anonymously", payload.Anonymously);
        fields.AddField("WebAppLink", payload.WebAppLink);
        fields.AddField("UserId", payload.UserId);
        fields.AddField("TelegramUserId", payload.TelegramUserId.ToString());
        if (!string.IsNullOrEmpty(telegramUsername))
        {
            fields.AddField("TelegramUsername", telegramUsername);
        }
        fields.AddField("CreatedAt", createdAt);
        fields.AddField("SentAt", sentAt);

        var response = await _airtableRepository.CreateRecord(_donationTableName, fields);

        if (!response.Success)
        {
            _logger.LogError($"Error creating donation record: {response.AirtableApiError.ErrorMessage}");
            throw new Exception($"Error creating donation record: {response.AirtableApiError.ErrorMessage}");
        }

        _logger.LogInformation($"Donation record created with ID: {response.Record.Id}");

        // Send Telegram notification
        await SendDonationNotification(payload, response.Record.Id);

        // Return DTO
        var donationDto = new DonationDto
        {
            RecordId = response.Record.Id,
            WebhookName = webhookName,
            DonationRequestId = payload.DonationRequestId,
            DonationName = payload.DonationName,
            Period = payload.Period,
            Amount = payload.Amount,
            Currency = payload.Currency,
            Anonymously = payload.Anonymously,
            WebAppLink = payload.WebAppLink,
            UserId = payload.UserId,
            TelegramUserId = payload.TelegramUserId,
            TelegramUsername = telegramUsername,
            CreatedAt = createdAt,
            SentAt = sentAt
        };

        return donationDto;
    }

    public async Task<SubscriptionDto> ProcessCancelledSubscription(string webhookName, CancelledSubscriptionPayload payload, DateTime createdAt, DateTime sentAt)
    {
        _logger.LogInformation($"Processing webhook '{webhookName}': Cancelled subscription {payload.SubscriptionName} for user {payload.TelegramUserId}");

        // Get Telegram username
        var telegramUsername = await GetTelegramUsername(payload.TelegramUserId);

        // Create Airtable record
        var fields = new Fields();
        fields.AddField("WebhookName", webhookName);
        fields.AddField("Type", webhookName);
        fields.AddField("SubscriptionName", payload.SubscriptionName);
        fields.AddField("SubscriptionId", payload.SubscriptionId);
        fields.AddField("PeriodId", payload.PeriodId);
        fields.AddField("Period", payload.Period);
        fields.AddField("Price", payload.Price);
        fields.AddField("Amount", payload.Amount);
        fields.AddField("Currency", payload.Currency);
        fields.AddField("UserId", payload.UserId);
        fields.AddField("TelegramUserId", payload.TelegramUserId.ToString());
        if (!string.IsNullOrEmpty(telegramUsername))
        {
            fields.AddField("TelegramUsername", telegramUsername);
        }
        fields.AddField("ChannelId", payload.ChannelId);
        fields.AddField("ChannelName", payload.ChannelName);
        fields.AddField("ExpiresAt", payload.ExpiresAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
        fields.AddField("CreatedAt", createdAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
        fields.AddField("SentAt", sentAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));

        var response = await _airtableRepository.CreateRecord(_subscriptionTableName, fields);

        if (!response.Success)
        {
            _logger.LogError($"Error creating cancelled subscription record: {response.AirtableApiError.ErrorMessage}");
            throw new Exception($"Error creating cancelled subscription record: {response.AirtableApiError.ErrorMessage}");
        }

        _logger.LogInformation($"Cancelled subscription record created with ID: {response.Record.Id}");

        // Send Telegram notification
        await SendCancelledSubscriptionNotification(payload, response.Record.Id);

        // Return DTO
        var subscriptionDto = new SubscriptionDto
        {
            RecordId = response.Record.Id,
            WebhookName = webhookName,
            SubscriptionName = payload.SubscriptionName,
            SubscriptionId = payload.SubscriptionId,
            PeriodId = payload.PeriodId,
            Period = payload.Period,
            Price = payload.Price,
            Amount = payload.Amount,
            Currency = payload.Currency,
            UserId = payload.UserId,
            TelegramUserId = payload.TelegramUserId,
            TelegramUsername = telegramUsername,
            ChannelId = payload.ChannelId,
            ChannelName = payload.ChannelName,
            ExpiresAt = payload.ExpiresAt,
            CreatedAt = createdAt,
            SentAt = sentAt
        };

        return subscriptionDto;
    }

    public async Task<DonationDto> ProcessCancelledDonation(string webhookName, CancelledDonationPayload payload, DateTime createdAt, DateTime sentAt)
    {
        _logger.LogInformation($"Processing webhook '{webhookName}': Cancelled donation {payload.DonationName} for user {payload.TelegramUserId}");

        // Get Telegram username
        var telegramUsername = await GetTelegramUsername(payload.TelegramUserId);

        // Create Airtable record
        var fields = new Fields();
        fields.AddField("WebhookName", webhookName);
        fields.AddField("Type", webhookName);
        fields.AddField("DonationRequestId", payload.DonationRequestId);
        fields.AddField("DonationName", payload.DonationName);
        fields.AddField("Period", payload.Period);
        fields.AddField("Amount", payload.Amount);
        fields.AddField("Currency", payload.Currency);
        fields.AddField("Anonymously", payload.Anonymously);
        fields.AddField("WebAppLink", payload.WebAppLink);
        fields.AddField("UserId", payload.UserId);
        fields.AddField("TelegramUserId", payload.TelegramUserId.ToString());
        if (!string.IsNullOrEmpty(telegramUsername))
        {
            fields.AddField("TelegramUsername", telegramUsername);
        }
        fields.AddField("CreatedAt", createdAt);
        fields.AddField("SentAt", sentAt);

        var response = await _airtableRepository.CreateRecord(_donationTableName, fields);

        if (!response.Success)
        {
            _logger.LogError($"Error creating cancelled donation record: {response.AirtableApiError.ErrorMessage}");
            throw new Exception($"Error creating cancelled donation record: {response.AirtableApiError.ErrorMessage}");
        }

        _logger.LogInformation($"Cancelled donation record created with ID: {response.Record.Id}");

        // Send Telegram notification
        await SendCancelledDonationNotification(payload, response.Record.Id);

        // Return DTO
        var donationDto = new DonationDto
        {
            RecordId = response.Record.Id,
            WebhookName = webhookName,
            DonationRequestId = payload.DonationRequestId,
            DonationName = payload.DonationName,
            Period = payload.Period,
            Amount = payload.Amount,
            Currency = payload.Currency,
            Anonymously = payload.Anonymously,
            WebAppLink = payload.WebAppLink,
            UserId = payload.UserId,
            TelegramUserId = payload.TelegramUserId,
            TelegramUsername = telegramUsername,
            CreatedAt = createdAt,
            SentAt = sentAt
        };

        return donationDto;
    }

    public async Task<IEnumerable<SubscriptionDto>> GetSubscriptions()
    {
        _logger.LogInformation("Retrieving all subscriptions");

        var response = await _airtableRepository.ListRecords<AtTributeSubscription>(_subscriptionTableName);

        if (!response.Success)
        {
            _logger.LogError($"Error retrieving subscriptions: {response.AirtableApiError.ErrorMessage}");
            throw new Exception($"Error retrieving subscriptions: {response.AirtableApiError.ErrorMessage}");
        }

        var subscriptions = response.Records.Select(r =>
        {
            var subscription = r.Fields;
            subscription.RecordId = r.Id;
            return TributeConverter.ToDto(subscription);
        });

        return subscriptions;
    }

    public async Task<IEnumerable<DonationDto>> GetDonations()
    {
        _logger.LogInformation("Retrieving all donations");

        var response = await _airtableRepository.ListRecords<AtTributeDonation>(_donationTableName);

        if (!response.Success)
        {
            _logger.LogError($"Error retrieving donations: {response.AirtableApiError.ErrorMessage}");
            throw new Exception($"Error retrieving donations: {response.AirtableApiError.ErrorMessage}");
        }

        var donations = response.Records.Select(r =>
        {
            var donation = r.Fields;
            donation.RecordId = r.Id;
            return TributeConverter.ToDto(donation);
        });

        return donations;
    }

    private async Task SendSubscriptionNotification(NewSubscriptionPayload payload, string recordId)
    {
        try
        {
            var superchatId = _configuration["Finance:Telegram:SuperchatId"];
            var topicId = _configuration["Finance:Telegram:TopicId"];

            if (string.IsNullOrEmpty(superchatId) || string.IsNullOrEmpty(topicId))
            {
                _logger.LogWarning("Telegram superchat or topic ID not configured, skipping notification");
                return;
            }

            var chatId = long.Parse(superchatId);
            var threadId = int.Parse(topicId);
            
            _logger.LogInformation($"Attempting to send subscription notification - ChatId: {chatId}, TopicId: {threadId}, RecordId: {recordId}");

            var message = $"🎉 New Subscription! {payload.Amount / 100.0:F2}{payload.Currency} per {payload.Period}";

            await _telegramBotClient.Client.SendMessage(
                chatId: chatId,
                text: message,
                parseMode: ParseMode.Html,
                messageThreadId: threadId
            );

            _logger.LogInformation($"Subscription notification sent successfully - ChatId: {chatId}, TopicId: {threadId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send subscription notification to Telegram");
        }
    }

    private async Task SendDonationNotification(RecurrentDonationPayload payload, string recordId)
    {
        try
        {
            var superchatId = _configuration["Finance:Telegram:SuperchatId"];
            var topicId = _configuration["Finance:Telegram:TopicId"];

            if (string.IsNullOrEmpty(superchatId) || string.IsNullOrEmpty(topicId))
            {
                _logger.LogWarning("Telegram superchat or topic ID not configured, skipping notification");
                return;
            }

            var chatId = long.Parse(superchatId);
            var threadId = int.Parse(topicId);
            
            _logger.LogInformation($"Attempting to send donation notification - ChatId: {chatId}, TopicId: {threadId}, RecordId: {recordId}");

            var message = $"💝 New Donation! {payload.Amount / 100.0:F2}{payload.Currency} {payload.Period?.ToLower()}";

            await _telegramBotClient.Client.SendMessage(
                chatId: chatId,
                text: message,
                parseMode: ParseMode.Html,
                messageThreadId: threadId
            );

            _logger.LogInformation($"Donation notification sent successfully - ChatId: {chatId}, TopicId: {threadId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send donation notification to Telegram");
        }
    }

    private async Task SendNewDonationNotification(NewDonationPayload payload, string recordId)
    {
        try
        {
            var superchatId = _configuration["Finance:Telegram:SuperchatId"];
            var topicId = _configuration["Finance:Telegram:TopicId"];

            if (string.IsNullOrEmpty(superchatId) || string.IsNullOrEmpty(topicId))
            {
                _logger.LogWarning("Telegram superchat or topic ID not configured, skipping notification");
                return;
            }

            var chatId = long.Parse(superchatId);
            var threadId = int.Parse(topicId);
            
            _logger.LogInformation($"Attempting to send new donation notification - ChatId: {chatId}, TopicId: {threadId}, RecordId: {recordId}");
            
            var message = $"💝 New Donation! {payload.Amount / 100.0:F2}{payload.Currency} {payload.Period?.ToLower()}";
            if (!string.IsNullOrEmpty(payload.Message))
            {
                message += $"\nMessage: {payload.Message}";
            }

            await _telegramBotClient.Client.SendMessage(
                chatId: chatId,
                text: message,
                parseMode: ParseMode.Html,
                messageThreadId: threadId
            );

            _logger.LogInformation($"New donation notification sent successfully - ChatId: {chatId}, TopicId: {threadId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send new donation notification to Telegram");
        }
    }

    private async Task SendCancelledSubscriptionNotification(CancelledSubscriptionPayload payload, string recordId)
    {
        try
        {
            var superchatId = _configuration["Finance:Telegram:SuperchatId"];
            var topicId = _configuration["Finance:Telegram:TopicId"];

            if (string.IsNullOrEmpty(superchatId) || string.IsNullOrEmpty(topicId))
            {
                _logger.LogWarning("Telegram superchat or topic ID not configured, skipping notification");
                return;
            }

            var chatId = long.Parse(superchatId);
            var threadId = int.Parse(topicId);
            
            _logger.LogInformation($"Attempting to send cancelled subscription notification - ChatId: {chatId}, TopicId: {threadId}, RecordId: {recordId}");

            var telegramUsername = await GetTelegramUsername(payload.TelegramUserId);
            var message = $"❌ Subscription Cancelled! {payload.Amount / 100.0:F2}{payload.Currency} per {payload.Period} by user {telegramUsername}";
            if (!string.IsNullOrEmpty(payload.CancelReason))
            {
                message += $"\nReason: {payload.CancelReason}";
            }

            await _telegramBotClient.Client.SendMessage(
                chatId: chatId,
                text: message,
                parseMode: ParseMode.Html,
                messageThreadId: threadId
            );

            _logger.LogInformation($"Cancelled subscription notification sent successfully - ChatId: {chatId}, TopicId: {threadId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send cancelled subscription notification to Telegram");
        }
    }

    private async Task SendCancelledDonationNotification(CancelledDonationPayload payload, string recordId)
    {
        try
        {
            var superchatId = _configuration["Finance:Telegram:SuperchatId"];
            var topicId = _configuration["Finance:Telegram:TopicId"];

            if (string.IsNullOrEmpty(superchatId) || string.IsNullOrEmpty(topicId))
            {
                _logger.LogWarning("Telegram superchat or topic ID not configured, skipping notification");
                return;
            }

            var chatId = long.Parse(superchatId);
            var threadId = int.Parse(topicId);
            
            _logger.LogInformation($"Attempting to send cancelled donation notification - ChatId: {chatId}, TopicId: {threadId}, RecordId: {recordId}");

            var telegramUsername = await GetTelegramUsername(payload.TelegramUserId);
            var message = $"❌ Donation Cancelled! {payload.Amount / 100.0:F2}{payload.Currency} per {payload.Period} by user {telegramUsername}";

            await _telegramBotClient.Client.SendMessage(
                chatId: chatId,
                text: message,
                parseMode: ParseMode.Html,
                messageThreadId: threadId
            );

            _logger.LogInformation($"Cancelled donation notification sent successfully - ChatId: {chatId}, TopicId: {threadId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send cancelled donation notification to Telegram");
        }
    }

    private async Task<string?> GetTelegramUsername(long telegramUserId)
    {
        try
        {
            var user = await _telegramBotClient.Client.GetChat(telegramUserId);
            return user.Username;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, $"Failed to get Telegram user info for ID {telegramUserId}");
            return null;
        }
    }
}

