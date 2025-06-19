using AirtableApiClient;

namespace Catebi.Api.Domain.Features.AdoptionBot;

[Obsolete("Use Admin and Cat services instead")]
public interface IAdoptionBotActionService
{
    /// <summary>
    /// Confirm a user's account and notify them.
    /// </summary>
    Task<bool> ConfirmUser(string atUserId);

    /// <summary>
    /// Add a cat photo record and notify the user.
    /// </summary>
    /// <param name="catRecordId">The ID of the cat record.</param>
    /// <param name="fileUrl">The URL of the cat photo file.</param>
    /// <returns>True if the operation was successful, otherwise false.</returns>
    Task<bool> AddCatPhoto(string catRecordId, string photoUrl);

    /// <summary>
    /// Add a cat payment record and notify the user.
    /// </summary>
    /// <param name="catRecordId">The ID of the cat record.</param>
    /// <param name="fileUrl">The URL of the payment confirmation file.</param>
    /// <returns>True if the operation was successful, otherwise false.</returns>
    Task<bool> AddCatPayment(string catRecordId, string fileUrl);

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
