namespace Catebi.Api.Models;

public class AddCatRequest
{
    public string OwnerRecordId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Sex { get; set; } = string.Empty;
    public string DateOfBirth { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool? IsVaccinatedComplex { get; set; }
    public bool? IsVaccinatedRabies { get; set; }
    public bool IsCatebiCat { get; set; }
}