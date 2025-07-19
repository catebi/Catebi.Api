namespace Catebi.Api.Domain.Features.AdoptionBot.ViewModels;

public class GetCatsForAdminRequest
{
    public string? Status { get; set; }
    public string? TextFilter { get; set; }
    public bool? PaidFilter { get; set; }
    public bool? FreeFilter { get; set; }
    public bool? IsCatebiFilter { get; set; }
    public string? Offset { get; set; }
}
