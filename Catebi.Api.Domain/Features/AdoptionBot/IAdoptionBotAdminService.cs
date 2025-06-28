using Catebi.Api.Domain.Features.AdoptionBot.ViewModels;

namespace Catebi.Api.Domain.Features.AdoptionBot;

public interface IAdoptionBotAdminService
{
    /// <summary>
    /// Confirm a user's account with additional fields (IsVolunteer, Notes) and notify them.
    /// </summary>
    Task<bool> ConfirmUser(string atUserId, bool isVolunteer, string? notes);

    /// <summary>
    /// Confirm a user's cat payment and grant access to paid features for exact cat.
    /// </summary>
    Task<bool> ConfirmCatPayment(string atCatId);

    /// <summary>
    /// Send a formatted message to all confirmed users
    /// </summary>
    Task<MessageDto> BroadcastMessage(string content, string adminRecordId);

    /// <summary>
    /// Get all broadcast messages
    /// </summary>
    Task<IEnumerable<MessageDto>> GetBroadcastMessages();

    /// <summary>
    /// Get all users with ToConfirm status
    /// </summary>
    Task<IEnumerable<UserDto>> GetUsersToConfirm();
}
