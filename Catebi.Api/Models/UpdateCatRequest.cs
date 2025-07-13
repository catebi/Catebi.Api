namespace Catebi.Api.Models;

public class UpdateCatRequest
{
    public string? RecordId { get; set; }
    public string OwnerRecordId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Sex { get; set; } = string.Empty;
    public string DateOfBirth { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool? IsVaccinatedComplex { get; set; }
    public bool? IsVaccinatedRabies { get; set; }
    public string? OwnerNotes { get; set; }
}
