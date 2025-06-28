namespace Catebi.Api.Domain.Features.AdoptionBot.ViewModels;

public class CatPaymentDto
{
    public string? RecordId { get; set; }
    public string CatRecordId { get; set; }
    public string? OwnerRecordId { get; set; }
    public AttachmentDto? Proof { get; set; }
    public string Status { get; set; }
    public string Created { get; set; }
    public string? CatName { get; set; }
}
