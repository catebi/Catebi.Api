using Catebi.Api.Domain.Features.Finance.Tribute.Models;
using Catebi.Api.Domain.Features.Finance.Tribute.ViewModels;

namespace Catebi.Api.Domain.Features.Finance.Tribute;

public interface ITributeService
{
    /// <summary>
    /// Process a new subscription webhook payload
    /// </summary>
    Task<SubscriptionDto> ProcessNewSubscription(string webhookName, NewSubscriptionPayload payload, DateTime createdAt, DateTime sentAt);

    /// <summary>
    /// Process a recurrent donation webhook payload
    /// </summary>
    Task<DonationDto> ProcessRecurrentDonation(string webhookName, RecurrentDonationPayload payload, DateTime createdAt, DateTime sentAt);

    /// <summary>
    /// Get all subscriptions from Airtable
    /// </summary>
    Task<IEnumerable<SubscriptionDto>> GetSubscriptions();

    /// <summary>
    /// Get all donations from Airtable
    /// </summary>
    Task<IEnumerable<DonationDto>> GetDonations();
}

