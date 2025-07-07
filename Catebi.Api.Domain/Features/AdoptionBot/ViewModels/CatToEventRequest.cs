namespace Catebi.Api.Domain.Features.AdoptionBot.ViewModels;

public class CatToEventRequest
{
    public string UserRecordId { get; set; } = string.Empty;
    public string CatRecordId { get; set; } = string.Empty;
    public string EventRecordId { get; set; } = string.Empty;
} 