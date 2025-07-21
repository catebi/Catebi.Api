using Catebi.Api.Domain.Features.AdoptionBot.Enums;
using System.Text.Json.Serialization;

namespace Catebi.Api.Domain.Features.AdoptionBot.Models;

public class AtEventShort
{
    [JsonIgnore]
    public string? RecordId { get; set; }
    public int EventId { get; set; }

    [JsonPropertyName("Status")]
    public string StatusValue { get; set; }
}
