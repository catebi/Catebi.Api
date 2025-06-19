namespace Catebi.Api.Domain.Features.AdoptionBot;

public interface IAdoptionBotAdminService
{
    /// <summary>
    /// Confirm a user's account and notify them.
    /// </summary>
    Task<bool> ConfirmUser(string atUserId);

    /// <summary>
    /// Confirm a user's cat payment and grant access to paid features for exact cat.
    /// </summary>
    Task<bool> ConfirmCatPayment(string atCatId);
} 