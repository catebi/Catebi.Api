using Catebi.Api.Domain.Features.AdoptionBot.Enums;
using System.Text.Json.Serialization;

namespace Catebi.Api.Domain.Features.AdoptionBot.Models;

public class AtCatbook
{
    public string? RecordId { get; set; }
    public int CatbookId { get; set; }
    public string? Descr { get; set; }
    public string? Color { get; set; }
    public string? Aliases { get; set; }
    public string? MediaLink { get; set; }
    public string? HealthNotes { get; set; }
    public bool? HasPassport { get; set; }
    public string? CharacterNotes { get; set; }
    public string? HistoryNotes { get; set; }
    public string? Location { get; set; }
    public bool? DeliveryAvailable { get; set; }
    public string? DeliveryNotes { get; set; }
    public string? ContactTg { get; set; }
    public string? CatbookLink { get; set; }
    public int? CatbookPostId { get; set; }
    public DateTime? Created { get; set; }
    public DateTime? Updated { get; set; }

    [JsonPropertyName("CatName")]
    public string[] CatNameValue { get; set; } = [];

    [JsonPropertyName("Status")]
    public string StatusValue { get; set; } = CatbookStatuses.ToConfirm.ToString();

    [JsonPropertyName("Cat")]
    public string[] CatValue { get; set; } = [];

    [JsonIgnore]
    public CatbookStatuses Status => Enum.Parse<CatbookStatuses>(StatusValue);

    [JsonIgnore]
    public string? CatRecordId => CatValue.FirstOrDefault();

    [JsonIgnore]
    public string? CatName => CatNameValue.FirstOrDefault();
}
