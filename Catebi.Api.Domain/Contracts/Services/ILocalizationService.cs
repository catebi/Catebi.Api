using Catebi.Api.Domain.Features.AdoptionBot.Enums;

namespace Catebi.Api.Domain.Contracts.Services;

public interface ILocalizationService
{
    /// <summary>
    /// Get localized message for user confirmation
    /// </summary>
    string GetUserConfirmationMessage(Languages language, string userName, bool isVolunteer);

    /// <summary>
    /// Get localized message for cat payment confirmation
    /// </summary>
    string GetCatPaymentConfirmationMessage(Languages language, string ownerName, string catName);

    /// <summary>
    /// Get localized message for event opening notification
    /// </summary>
    string GetEventOpenNotificationMessage(Languages language, string eventName, DateTime eventDate, string eventLocation, string eventDescription);

    /// <summary>
    /// Get localized message for general broadcast (preserves original formatting)
    /// </summary>
    string GetBroadcastMessage(Languages language, string content);

    /// <summary>
    /// Get localized admin notification message for new user registration
    /// </summary>
    string GetAdminUserRegistrationNotification(Languages language, string userName, string userTelegram, string userRecordId);

    /// <summary>
    /// Get localized admin notification message for new payment submission
    /// </summary>
    string GetAdminPaymentSubmissionNotification(Languages language, string catName, string ownerName, string catRecordId, string paymentRecordId);

    /// <summary>
    /// Get admin notification message for cat adoption
    /// </summary>
    string GetAdminCatAdoptionNotification(Languages language, string catName, string ownerName, string? adoptionComment = null, string? actionByUserName = null, string? actionByUserTelegram = null);

    /// <summary>
    /// Get localized message for cat registration to event (for user)
    /// </summary>
    string GetCatRegisteredToEventMessage(Languages language, string catName, string eventName, DateTime eventDate, string eventLocation);

    /// <summary>
    /// Get localized message for cat removal from event (for user)
    /// </summary>
    string GetCatRemovedFromEventMessage(Languages language, string catName, string eventName, DateTime eventDate, string eventLocation);

    /// <summary>
    /// Get admin notification message for cat registered to event
    /// </summary>
    string GetAdminCatRegisteredToEventNotification(Languages language, string catName, string ownerName, string catRecordId, string eventName, string eventRecordId, string? actionByUserName = null, string? actionByUserTelegram = null);

    /// <summary>
    /// Get admin notification message for cat removed from event
    /// </summary>
    string GetAdminCatRemovedFromEventNotification(Languages language, string catName, string ownerName, string catRecordId, string eventName, string eventRecordId, string? actionByUserName = null, string? actionByUserTelegram = null);
} 