using System.Text.Json.Serialization;

namespace Catebi.Api.Domain.Features.Finance.Tribute.Models;

public class AtTributeDonation
{
    public string? RecordId { get; set; }

    [JsonPropertyName("WebhookName")]
    public string WebhookName { get; set; } = string.Empty;

    [JsonPropertyName("DonationRequestId")]
    public int DonationRequestId { get; set; }

    [JsonPropertyName("DonationName")]
    public string DonationName { get; set; } = string.Empty;

    [JsonPropertyName("Period")]
    public string Period { get; set; } = string.Empty;

    [JsonPropertyName("Amount")]
    public int Amount { get; set; }

    [JsonPropertyName("Currency")]
    public string Currency { get; set; } = string.Empty;

    [JsonPropertyName("Anonymously")]
    public bool Anonymously { get; set; }

    [JsonPropertyName("WebAppLink")]
    public string WebAppLink { get; set; } = string.Empty;

    [JsonPropertyName("UserId")]
    public int UserId { get; set; }

    [JsonPropertyName("TelegramUserId")]
    public long TelegramUserId { get; set; }

    [JsonPropertyName("CreatedAt")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("SentAt")]
    public DateTime SentAt { get; set; }
}

