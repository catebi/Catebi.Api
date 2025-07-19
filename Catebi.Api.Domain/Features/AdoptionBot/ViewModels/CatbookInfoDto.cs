namespace Catebi.Api.Domain.Features.AdoptionBot.ViewModels;

public class CatbookInfoDto
{
    public string? RecordId { get; set; }
    public int? CatbookId { get; set; }
    public string? CatRecordId { get; set; }
    public string? CatName { get; set; }
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
    public string? Status { get; set; }
    public string? CatbookLink { get; set; }
    public int? CatbookPostId { get; set; }
    public string? Created { get; set; }
    public string? Updated { get; set; }
}
