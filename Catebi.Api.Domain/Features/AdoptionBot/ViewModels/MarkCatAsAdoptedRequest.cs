namespace Catebi.Api.Domain.Features.AdoptionBot.ViewModels;

public class MarkCatAsAdoptedRequest
{
    public string CatRecordId { get; set; } = string.Empty;
    public string UserRecordId { get; set; } = string.Empty;
    public string? AdoptionComment { get; set; }
} 