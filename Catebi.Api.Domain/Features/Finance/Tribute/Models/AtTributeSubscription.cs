using System.Text.Json.Serialization;

namespace Catebi.Api.Domain.Features.Finance.Tribute.Models;

public class AtTributeSubscription
{
    public string? RecordId { get; set; }

    [JsonPropertyName("SubscriptionName")]
    public string SubscriptionName { get; set; } = string.Empty;

    [JsonPropertyName("SubscriptionId")]
    public int SubscriptionId { get; set; }

    [JsonPropertyName("PeriodId")]
    public int PeriodId { get; set; }

    [JsonPropertyName("Period")]
    public string Period { get; set; } = string.Empty;

    [JsonPropertyName("Price")]
    public int Price { get; set; }

    [JsonPropertyName("Amount")]
    public int Amount { get; set; }

    [JsonPropertyName("Currency")]
    public string Currency { get; set; } = string.Empty;

    [JsonPropertyName("UserId")]
    public int UserId { get; set; }

    [JsonPropertyName("TelegramUserId")]
    public long TelegramUserId { get; set; }

    [JsonPropertyName("ChannelId")]
    public int ChannelId { get; set; }

    [JsonPropertyName("ChannelName")]
    public string ChannelName { get; set; } = string.Empty;

    [JsonPropertyName("ExpiresAt")]
    public DateTime ExpiresAt { get; set; }

    [JsonPropertyName("CreatedAt")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("SentAt")]
    public DateTime SentAt { get; set; }
}

