namespace Catebi.Api.Domain.Contracts.Services;

public interface IFreeganService
{
    Task<bool> SaveMessage(FreeganMessageDto message);
    Task<List<DonationChatDto>> GetDonationChats();
    Task<bool> SaveReaction(DonationMessageReactionDto reaction);

    Task<List<KeywordGroupDto>> GetSearchConfig();
}
