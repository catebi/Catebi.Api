using AirtableApiClient;
using Catebi.Api.Domain.Features.AdoptionBot;
using Catebi.Api.Domain.Features.Finance.Tribute.Converters;
using Catebi.Api.Domain.Features.Finance.Tribute.Enums;
using Catebi.Api.Domain.Features.Finance.Tribute.Models;
using Catebi.Api.Domain.Features.Finance.Tribute.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Catebi.Api.Domain.Features.Finance.Tribute;

public class TributeService : ITributeService
{
    private readonly AirtableBase _airtableBase;
    private readonly AirtableRepository _airtableRepository;
    private readonly CommonTelegramBotClient _telegramBotClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TributeService> _logger;
    private readonly string _subscriptionTableName = TributeAirTables.TributeSubscription.ToString();
    private readonly string _donationTableName = TributeAirTables.TributeDonation.ToString();

    public TributeService(
        [FromKeyedServices("Finance")] AirtableBase airtableBase,
        CommonTelegramBotClient telegramBotClient,
        IConfiguration configuration,
        ILogger<TributeService> logger,
        ILogger<AirtableRepository> airtableLogger)
    {
        _airtableBase = airtableBase;
        _airtableRepository = new AirtableRepository(airtableBase, airtableLogger);
        _telegramBotClient = telegramBotClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<SubscriptionDto> ProcessNewSubscription(string webhookName, NewSubscriptionPayload payload, DateTime createdAt, DateTime sentAt)
    {
        _logger.LogInformation($"Processing webhook '{webhookName}': {payload.SubscriptionName} for user {payload.TelegramUserId}");

        // Create Airtable record
        var fields = new Fields();
        fields.AddField("WebhookName", webhookName);
        fields.AddField("SubscriptionName", payload.SubscriptionName);
        fields.AddField("SubscriptionId", payload.SubscriptionId);
        fields.AddField("PeriodId", payload.PeriodId);
        fields.AddField("Period", payload.Period);
        fields.AddField("Price", payload.Price);
        fields.AddField("Amount", payload.Amount);
        fields.AddField("Currency", payload.Currency);
        fields.AddField("UserId", payload.UserId);
        fields.AddField("TelegramUserId", payload.TelegramUserId.ToString());
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
            ChannelId = payload.ChannelId,
            ChannelName = payload.ChannelName,
            ExpiresAt = payload.ExpiresAt,
            CreatedAt = createdAt,
            SentAt = sentAt
        };

        return subscriptionDto;
    }

    public async Task<DonationDto> ProcessRecurrentDonation(string webhookName, RecurrentDonationPayload payload, DateTime createdAt, DateTime sentAt)
    {
        _logger.LogInformation($"Processing webhook '{webhookName}': {payload.DonationName} for user {payload.TelegramUserId}");

        // Create Airtable record
        var fields = new Fields();
        fields.AddField("WebhookName", webhookName);
        fields.AddField("DonationRequestId", payload.DonationRequestId);
        fields.AddField("DonationName", payload.DonationName);
        fields.AddField("Period", payload.Period);
        fields.AddField("Amount", payload.Amount);
        fields.AddField("Currency", payload.Currency);
        fields.AddField("Anonymously", payload.Anonymously);
        fields.AddField("WebAppLink", payload.WebAppLink);
        fields.AddField("UserId", payload.UserId);
        fields.AddField("TelegramUserId", payload.TelegramUserId.ToString());
        fields.AddField("CreatedAt", createdAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
        fields.AddField("SentAt", sentAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));

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

            var message = $"🎉 <b>New Subscription!</b>\n\n" +
                         $"📝 <b>Name:</b> {payload.SubscriptionName}\n" +
                         $"💰 <b>Amount:</b> {payload.Amount / 100.0:F2} {payload.Currency.ToUpper()}\n" +
                         $"⏰ <b>Period:</b> {payload.Period}\n" +
                         $"👤 <b>User ID:</b> {payload.UserId}\n" +
                         $"📱 <b>Telegram ID:</b> {payload.TelegramUserId}\n" +
                         $"📺 <b>Channel:</b> {payload.ChannelName}\n" +
                         $"📅 <b>Expires:</b> {payload.ExpiresAt:yyyy-MM-dd HH:mm}\n" +
                         $"🆔 <b>Record ID:</b> {recordId}";

            await _telegramBotClient.Client.SendMessage(
                chatId: long.Parse(superchatId),
                text: message,
                parseMode: ParseMode.Html,
                messageThreadId: int.Parse(topicId)
            );

            _logger.LogInformation($"Subscription notification sent to Telegram");
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

            var donorInfo = payload.Anonymously ? "Anonymous" : $"User {payload.UserId} (TG: {payload.TelegramUserId})";

            var message = $"💝 <b>New Donation!</b>\n\n" +
                         $"📝 <b>Name:</b> {payload.DonationName}\n" +
                         $"💰 <b>Amount:</b> {payload.Amount / 100.0:F2} {payload.Currency.ToUpper()}\n" +
                         $"⏰ <b>Period:</b> {payload.Period}\n" +
                         $"👤 <b>Donor:</b> {donorInfo}\n" +
                         $"🔗 <b>Link:</b> {payload.WebAppLink}\n" +
                         $"🆔 <b>Record ID:</b> {recordId}";

            await _telegramBotClient.Client.SendMessage(
                chatId: long.Parse(superchatId),
                text: message,
                parseMode: ParseMode.Html,
                messageThreadId: int.Parse(topicId)
            );

            _logger.LogInformation($"Donation notification sent to Telegram");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send donation notification to Telegram");
        }
    }
}

