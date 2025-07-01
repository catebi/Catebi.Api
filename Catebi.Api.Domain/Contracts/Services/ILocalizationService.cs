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
    /// Get localized message for cat photo added
    /// </summary>
    string GetCatPhotoAddedMessage(Languages language, string ownerName, string catName);

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
} 