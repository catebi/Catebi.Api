using Catebi.Api.Domain.Features.AdoptionBot.Enums;
using System.Text.Json.Serialization;

namespace Catebi.Api.Domain.Features.AdoptionBot.Models;

public class AtCatPaymentShort
{
    public string? RecordId { get; set; }

    [JsonPropertyName("Status")]
    public string StatusValue { get; set; } = string.Empty;

    [JsonIgnore]
    public CatPaymentStatuses Status => Enum.Parse<CatPaymentStatuses>(StatusValue);
}
