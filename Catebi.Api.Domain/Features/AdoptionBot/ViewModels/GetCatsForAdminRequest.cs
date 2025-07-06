using Catebi.Api.Domain.Features.AdoptionBot.Enums;

namespace Catebi.Api.Domain.Features.AdoptionBot.ViewModels;

public class GetCatsForAdminRequest
{
    public string? Status { get; set; }
    public string? CatNameFilter { get; set; }    
    public string? Offset { get; set; }
} 