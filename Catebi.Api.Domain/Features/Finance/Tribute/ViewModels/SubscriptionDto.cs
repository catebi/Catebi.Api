namespace Catebi.Api.Domain.Features.Finance.Tribute.ViewModels;

public class SubscriptionDto
{
    public string? RecordId { get; set; }
    public string SubscriptionName { get; set; } = string.Empty;
    public int SubscriptionId { get; set; }
    public int PeriodId { get; set; }
    public string Period { get; set; } = string.Empty;
    public int Price { get; set; }
    public int Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public int UserId { get; set; }
    public long TelegramUserId { get; set; }
    public int ChannelId { get; set; }
    public string ChannelName { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime SentAt { get; set; }
}

