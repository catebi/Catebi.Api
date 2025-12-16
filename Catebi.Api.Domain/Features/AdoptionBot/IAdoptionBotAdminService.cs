namespace Catebi.Api.Domain.Features.AdoptionBot;

public interface IAdoptionBotAdminService
{
    Task<bool> ConfirmUser(string atUserId, bool isVolunteer, string? notes, string? confirmedByTelegramUsername);
    Task<bool> ConfirmCatPayment(string paymentRecordId);
    Task<MessageDto> BroadcastMessage(string content, string adminRecordId);
    Task<IEnumerable<MessageDto>> GetBroadcastMessages();
    Task<IEnumerable<UserDto>> GetUsersToConfirm();
    Task<IEnumerable<CatPaymentDto>> GetPaymentsToConfirm();
    Task<bool> NotifyAdminsAboutUserRegistration(string userName, string userTelegram, string userRecordId);
    Task<bool> NotifyAdminsAboutPaymentSubmission(string catName, string ownerName, string catRecordId, string paymentRecordId);
    Task<bool> NotifyAdminsAboutCatAdoption(string catName, string ownerName, string catRecordId, string? adoptionComment = null, string? actionByUserName = null, string? actionByUserTelegram = null);
    Task<bool> NotifyAdminsAboutCatRegisteredToEvent(string catName, string ownerName, string catRecordId, string eventName, string eventRecordId, string? actionByUserName = null, string? actionByUserTelegram = null);
    Task<bool> NotifyAdminsAboutCatRemovedFromEvent(string catName, string ownerName, string catRecordId, string eventName, string eventRecordId, string? actionByUserName = null, string? actionByUserTelegram = null);
    Task<IEnumerable<AtUser>> GetAdminUsers();
    Task<DashboardInfoDto> GetDashboardInfo();
}
