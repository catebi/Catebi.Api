namespace Catebi.Api.Domain.Features.Finance.Tribute.ViewModels;

public class DonationDto
{
    public string? RecordId { get; set; }
    public string WebhookName { get; set; } = string.Empty;
    public int DonationRequestId { get; set; }
    public string DonationName { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty;
    public int Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public bool Anonymously { get; set; }
    public string WebAppLink { get; set; } = string.Empty;
    public int UserId { get; set; }
    public long TelegramUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime SentAt { get; set; }
}

