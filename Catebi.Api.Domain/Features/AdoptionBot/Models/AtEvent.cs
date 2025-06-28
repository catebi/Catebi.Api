using Catebi.Api.Domain.Features.AdoptionBot.Enums;
using System.Text.Json.Serialization;

namespace Catebi.Api.Domain.Features.AdoptionBot.Models;

public class AtEvent
{
    [JsonIgnore]
    public string? RecordId { get; set; }

    public int EventId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime When { get; set; }
    public string Where { get; set; }
    public DateTime? Created { get; set; }
    public string[] Cats { get; set; } = [];
    public int PaidSlotCount { get; set; }
    public int FreeSlotCount { get; set; }
    public AtAttachment[] Poster { get; set; } = [];

    [JsonPropertyName("Status")]
    public string StatusValue { get; set; }

    [JsonIgnore]
    public EventStatuses Status => Enum.Parse<EventStatuses>(StatusValue);
}
