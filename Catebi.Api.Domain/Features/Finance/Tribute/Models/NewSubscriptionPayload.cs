using System.Text.Json.Serialization;

namespace Catebi.Api.Domain.Features.Finance.Tribute.Models;

public class NewSubscriptionPayload
{
    [JsonPropertyName("subscription_name")]
    public string SubscriptionName { get; set; } = string.Empty;

    [JsonPropertyName("subscription_id")]
    public int SubscriptionId { get; set; }

    [JsonPropertyName("period_id")]
    public int PeriodId { get; set; }

    [JsonPropertyName("period")]
    public string Period { get; set; } = string.Empty;

    [JsonPropertyName("price")]
    public int Price { get; set; }

    [JsonPropertyName("amount")]
    public int Amount { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = string.Empty;

    [JsonPropertyName("user_id")]
    public int UserId { get; set; }

    [JsonPropertyName("telegram_user_id")]
    public long TelegramUserId { get; set; }

    [JsonPropertyName("channel_id")]
    public int ChannelId { get; set; }

    [JsonPropertyName("channel_name")]
    public string ChannelName { get; set; } = string.Empty;

    [JsonPropertyName("expires_at")]
    public DateTime ExpiresAt { get; set; }
}

