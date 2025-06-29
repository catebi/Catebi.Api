namespace Catebi.Api.Domain.Features.AdoptionBot.ViewModels;

public class CatPaymentDto
{
    public string RecordId { get; set; }
    public string CatRecordId { get; set; }
    public string CatName { get; set; }
    public string OwnerName { get; set; }
    public string OwnerTelegram { get; set; }
    public int? Price { get; set; }
    public AttachmentDto? Proof { get; set; }
    public string? Status { get; set; }
    public string? Created { get; set; }
}
