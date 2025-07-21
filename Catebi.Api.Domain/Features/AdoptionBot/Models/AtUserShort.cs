using Catebi.Api.Domain.Features.AdoptionBot.Enums;
using System.Text.Json.Serialization;

namespace Catebi.Api.Domain.Features.AdoptionBot.Models;

public class AtUserShort
{
    public string? RecordId { get; set; }
    public int UserId { get; set; }

    [JsonPropertyName("Status")]
    public string StatusValue { get; set; }

    [JsonIgnore]
    public UserStatuses Status => Enum.Parse<UserStatuses>(StatusValue);
}
