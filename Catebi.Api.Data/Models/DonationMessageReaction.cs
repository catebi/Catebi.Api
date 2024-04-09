namespace Catebi.Api.Data.Models;

public class DonationMessageReactionDto(int messageId, string content, int likeCount, int dislikeCount)
{
    public int MessageId { get; set; } = messageId;
    public string Content { get; set; } = content;
    public int LikeCount { get; set; } = likeCount;
    public int DislikeCount { get; set; } = dislikeCount;
}