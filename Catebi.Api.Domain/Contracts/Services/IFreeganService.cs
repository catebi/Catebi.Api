namespace Catebi.Api.Domain.Contracts.Services;

public interface IFreeganService
{
    Task<bool> SaveMessage(FreeganMessageDto message);
    Task<List<DonationChatDto>> GetDonationChats();
}