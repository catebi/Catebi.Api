namespace Catebi.Api.Domain.Implementations.Services;

public class FreeganService(  IUnitOfWork unitOfWork,
                                     ILogger<FreeganService> logger
                                 ) : IFreeganService
{
    private readonly ILogger<FreeganService> _logger = logger;
    private readonly IFreeganMessageRepository _freeganRepo = unitOfWork.FreeganRepository;
    private readonly IDonationChatRepository _chatRepo = unitOfWork.DonationChatRepository;
    private readonly IDonationMessageReactionRepository _reactionRepo = unitOfWork.DonationMessageReactionRepository;

    public async Task<bool> SaveMessage(FreeganMessageDto message)
    {
        try
        {
            var freeganMessage = new Message
            {
                OriginalText = message.OriginalText,
                LemmatizedText = message.LemmatizedText,
                ChatLink = message.ChatLink,
                Accepted = message.Accepted
            };

            await _freeganRepo.InsertAsync(freeganMessage);
            await unitOfWork.SaveAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving message");
            return false;
        }
    }

    public async Task<List<DonationChatDto>> GetDonationChats()
    {
        var result = await _chatRepo.GetAsync(filter: x => x.IsActual);
        return result.Select(x => new DonationChatDto(x.DonationChatId, x.ChatUrl, x.IsConnected))
            .ToList();
    }

    public async Task<bool> SaveReaction(DonationMessageReactionDto reaction)
    {
        try
        {
            var donationMessageReaction = new DonationMessageReaction
            {
                MessageId = reaction.MessageId,
                Content = reaction.Content,
                LikeCount = reaction.LikeCount,
                DislikeCount = reaction.DislikeCount
            };

            await _reactionRepo.InsertAsync(donationMessageReaction);
            await unitOfWork.SaveAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving message");
            return false;
        }
    }

    #region Public



    #endregion

    #region Private

    #endregion

}
