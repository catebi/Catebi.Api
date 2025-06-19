namespace Catebi.Api.Domain.Features.AdoptionBot.ViewModels;

public class EventDto
{
    public string? RecordId { get; set; }
    public int? EventId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int CatSlotCount { get; set; }
    public int CatCount { get; set; }
    public string When { get; set; }
    public string Where { get; set; }
    public string? Created { get; set; }
    public List<string>? Cats { get; set; }
    public int PaidSlotCount { get; set; }
    public int FreeSlotCount { get; set; }
    public List<AttachmentDto>? Poster { get; set; }
    public string Status { get; set; }
} 