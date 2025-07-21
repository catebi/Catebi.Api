using Catebi.Api.Domain.Features.AdoptionBot.Enums;
using System.Text.Json.Serialization;

namespace Catebi.Api.Domain.Features.AdoptionBot.Models;

public class AtCatbookShort
{
    public string? RecordId { get; set; }
    public int CatbookId { get; set; }

    [JsonPropertyName("Status")]
    public string StatusValue { get; set; } = CatbookStatuses.ToConfirm.ToString();

    [JsonIgnore]
    public CatbookStatuses Status => Enum.Parse<CatbookStatuses>(StatusValue);
}
