using Catebi.Api.Domain.Features.AdoptionBot.Enums;
using System.Text.Json.Serialization;

namespace Catebi.Api.Domain.Features.AdoptionBot.Models;

public class AtCatShort
{
    public string? RecordId { get; set; }
    public int CatId { get; set; }

    [JsonPropertyName("Status")]
    public string StatusValue { get; set; }

    [JsonIgnore]
    public CatStatuses Status => Enum.Parse<CatStatuses>(StatusValue);
}
