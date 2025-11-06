using System.Text.Json;
using System.Text.Json.Serialization;

namespace Catebi.Api.Domain.Features.Finance.Tribute.Models;

public class TributeWebhookRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("sent_at")]
    public DateTime SentAt { get; set; }

    [JsonPropertyName("payload")]
    public JsonElement Payload { get; set; }
}

