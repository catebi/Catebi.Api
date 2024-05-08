namespace Catebi.Api.Domain.Implementations.Services;

public class FreeganService(  IUnitOfWork unitOfWork,
                                     ILogger<FreeganService> logger
                                 ) : IFreeganService
{
    private readonly ILogger<FreeganService> _logger = logger;
    private readonly IFreeganMessageRepository _freeganRepo = unitOfWork.FreeganRepository;
    private readonly IDonationChatRepository _chatRepo = unitOfWork.DonationChatRepository;
    private readonly IDonationMessageReactionRepository _reactionRepo = unitOfWork.DonationMessageReactionRepository;
    private readonly IKeywordGroupRepository _keywordGroupRepo = unitOfWork.KeywordGroupRepository;

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
            var donation = await _reactionRepo.SingleOrDefaultAsync(filter: x => x.MessageId.Equals(reaction.MessageId));

            if (donation != null) 
            {
                donation.LikeCount = reaction.LikeCount;
                donation.DislikeCount = reaction.DislikeCount;
                _reactionRepo.Update(donation);
            }
            else
            {
                var donationMessageReaction = new DonationMessageReaction
                {
                    MessageId = reaction.MessageId,
                    Content = reaction.Content,
                    LikeCount = reaction.LikeCount,
                    DislikeCount = reaction.DislikeCount
                };
                await _reactionRepo.InsertAsync(donationMessageReaction);
            }
            await unitOfWork.SaveAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving message");
            return false;
        }
    }

    public async Task<List<KeywordGroupDto>> GetSearchConfig()
    {
        var result = await _keywordGroupRepo.GetAsync
        (
            filter: x => x.IsActual,
            include: x => x.Include(y => y.Keyword)
                        .Include(y => y.GroupIncludedKeyword)
                        .Include(y => y.GroupExcludedKeyword)
        );
        Console.WriteLine(result);
        return result.Select(GetKeywordsDto).ToList();
    }

    #region Public



    #endregion

    #region Private

    private KeywordGroupDto GetKeywordsDto(Group group) =>
        new()
        {
            Name = group.Name,
            Keywords = group.Keyword.Select(kw => kw.Keyword1).ToList(),
            IncludeKeywords = group.GroupIncludedKeyword.Select(gik => gik.Keyword).ToList(),
            ExcludeKeywords = group.GroupExcludedKeyword.Select(gek => gek.Keyword).ToList(),
        };

    #endregion

}
