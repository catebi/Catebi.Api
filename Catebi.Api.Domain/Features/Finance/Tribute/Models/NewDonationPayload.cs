using System.Text.Json.Serialization;

namespace Catebi.Api.Domain.Features.Finance.Tribute.Models;

public class NewDonationPayload
{
    [JsonPropertyName("donation_request_id")]
    public int DonationRequestId { get; set; }

    [JsonPropertyName("donation_name")]
    public string DonationName { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("period")]
    public string Period { get; set; } = string.Empty;

    [JsonPropertyName("amount")]
    public int Amount { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = string.Empty;

    [JsonPropertyName("anonymously")]
    public bool Anonymously { get; set; }

    [JsonPropertyName("web_app_link")]
    public string WebAppLink { get; set; } = string.Empty;

    [JsonPropertyName("user_id")]
    public int UserId { get; set; }

    [JsonPropertyName("telegram_user_id")]
    public long TelegramUserId { get; set; }
}

