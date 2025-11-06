using Catebi.Api.Domain.Features.Finance.Tribute.Models;
using Catebi.Api.Domain.Features.Finance.Tribute.ViewModels;

namespace Catebi.Api.Domain.Features.Finance.Tribute.Converters;

public static class TributeConverter
{
    public static SubscriptionDto ToDto(AtTributeSubscription subscription) => new()
    {
        RecordId = subscription.RecordId,
        SubscriptionName = subscription.SubscriptionName,
        SubscriptionId = subscription.SubscriptionId,
        PeriodId = subscription.PeriodId,
        Period = subscription.Period,
        Price = subscription.Price,
        Amount = subscription.Amount,
        Currency = subscription.Currency,
        UserId = subscription.UserId,
        TelegramUserId = subscription.TelegramUserId,
        ChannelId = subscription.ChannelId,
        ChannelName = subscription.ChannelName,
        ExpiresAt = subscription.ExpiresAt,
        CreatedAt = subscription.CreatedAt,
        SentAt = subscription.SentAt
    };

    public static DonationDto ToDto(AtTributeDonation donation) => new()
    {
        RecordId = donation.RecordId,
        DonationRequestId = donation.DonationRequestId,
        DonationName = donation.DonationName,
        Period = donation.Period,
        Amount = donation.Amount,
        Currency = donation.Currency,
        Anonymously = donation.Anonymously,
        WebAppLink = donation.WebAppLink,
        UserId = donation.UserId,
        TelegramUserId = donation.TelegramUserId,
        CreatedAt = donation.CreatedAt,
        SentAt = donation.SentAt
    };

    public static NewSubscriptionPayload ToPayload(AtTributeSubscription subscription) => new()
    {
        SubscriptionName = subscription.SubscriptionName,
        SubscriptionId = subscription.SubscriptionId,
        PeriodId = subscription.PeriodId,
        Period = subscription.Period,
        Price = subscription.Price,
        Amount = subscription.Amount,
        Currency = subscription.Currency,
        UserId = subscription.UserId,
        TelegramUserId = subscription.TelegramUserId,
        ChannelId = subscription.ChannelId,
        ChannelName = subscription.ChannelName,
        ExpiresAt = subscription.ExpiresAt
    };

    public static RecurrentDonationPayload ToPayload(AtTributeDonation donation) => new()
    {
        DonationRequestId = donation.DonationRequestId,
        DonationName = donation.DonationName,
        Period = donation.Period,
        Amount = donation.Amount,
        Currency = donation.Currency,
        Anonymously = donation.Anonymously,
        WebAppLink = donation.WebAppLink,
        UserId = donation.UserId,
        TelegramUserId = donation.TelegramUserId
    };
}

