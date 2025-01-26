using AirtableApiClient;

namespace Catebi.Api.Domain.Features.AdoptionBot;

public interface IAdoptionBotActionService
{
    /// <summary>
    /// Confirm a user's account and notify them.
    /// </summary>
    Task<bool> ConfirmUser(string atUserId);

    /// <summary>
    /// Confirm a user's cat payment and grant access to paid features for exact cat.
    /// </summary>
    Task<bool> ConfirmCatPayment(string atCatId);

    /// <summary>
    /// Confirm a user that their cat has been placed in the catbook.
    /// </summary>
    Task<bool> ConfirmCatbookPlacement(string atCatId);

    /// <summary>
    /// Notify the bot to open event for a cat's registration.
    /// </summary>
    Task<bool> OpenEventRegistration(string atEventId);

    /// <summary>
    /// Notify the bot to close event after finishing registration.
    /// </summary>
    Task<bool> CloseEventRegistration(string atEventId);
}
