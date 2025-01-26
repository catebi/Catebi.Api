using Catebi.Api.Domain.Features.AdoptionBot.Enums;
using System.Text.Json.Serialization;

namespace Catebi.Api.Domain.Features.AdoptionBot.Models;

public class AtEvent
{
    public int EventId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int CatSlotCount { get; set; }
    public int CatCount { get; set; }

    [JsonPropertyName("Status")]
    public string StatusValue { get; set; }

    [JsonIgnore]
    public EventStatuses Status => Enum.Parse<EventStatuses>(StatusValue);
}
