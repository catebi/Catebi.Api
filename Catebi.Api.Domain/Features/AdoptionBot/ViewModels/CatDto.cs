namespace Catebi.Api.Domain.Features.AdoptionBot.ViewModels;

public class CatDto
{
    public int? CatId { get; set; }
    public string? RecordId { get; set; }
    public string OwnerRecordId { get; set; }
    public string OwnerTelegram { get; set; }
    public string Name { get; set; }
    public string Sex { get; set; } = string.Empty;
    public string DateOfBirth { get; set; }
    public List<AttachmentDto>? Photos { get; set; }
    public AttachmentDto? MainPhoto { get; set; }
    public string Status { get; set; }
    public bool? IsVaccinatedComplex { get; set; }
    public bool? IsVaccinatedRabies { get; set; }
    public string? OwnerNotes { get; set; }
    public bool OwnerIsVolunteer { get; set; }
    public int PaidAccountPrice { get; set; }
    public bool ConfirmedPaymentAccount { get; set; }
    public string Created { get; set; }
}
