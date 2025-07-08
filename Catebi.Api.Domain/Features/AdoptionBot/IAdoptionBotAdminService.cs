using Catebi.Api.Domain.Features.AdoptionBot.ViewModels;
using Catebi.Api.Domain.Features.AdoptionBot.Models;

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
    Task<bool> ConfirmCatPayment(string paymentRecordId);

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

    /// <summary>
    /// Get all payments to confirm
    /// </summary>
    Task<IEnumerable<CatPaymentDto>> GetPaymentsToConfirm();

    /// <summary>
    /// Send notification to all admin users about new user registration
    /// </summary>
    Task<bool> NotifyAdminsAboutUserRegistration(string userName, string userTelegram, string userRecordId);

    /// <summary>
    /// Send notification to all admin users about new payment submission
    /// </summary>
    Task<bool> NotifyAdminsAboutPaymentSubmission(string catName, string ownerName, string catRecordId, string paymentRecordId);

    /// <summary>
    /// Send notification to all admin users about cat adoption
    /// </summary>
    Task<bool> NotifyAdminsAboutCatAdoption(string catName, string ownerName, string catRecordId, string? adoptionComment = null, string? actionByUserName = null, string? actionByUserTelegram = null);

    /// <summary>
    /// Send notification to all admin users about cat registered to event
    /// </summary>
    Task<bool> NotifyAdminsAboutCatRegisteredToEvent(string catName, string ownerName, string catRecordId, string eventName, string eventRecordId, string? actionByUserName = null, string? actionByUserTelegram = null);

    /// <summary>
    /// Send notification to all admin users about cat removed from event
    /// </summary>
    Task<bool> NotifyAdminsAboutCatRemovedFromEvent(string catName, string ownerName, string catRecordId, string eventName, string eventRecordId, string? actionByUserName = null, string? actionByUserTelegram = null);

    /// <summary>
    /// Get all admin users from the system
    /// </summary>
    Task<IEnumerable<AtUser>> GetAdminUsers();
}
