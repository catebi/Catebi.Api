namespace Catebi.Map.Data.Models;

public class CatDto
{
    public int Id { get; set; }
    public string? NotionCatId { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? GeoLocation { get; set; }
    public string? NotionPageUrl { get; set; }
    public DateOnly? InDate { get; set; }
    public DateOnly? OutDate { get; set; }
    public DateOnly? NeuteredDate { get; set; }
    public string? Comment { get; set; }
    public VolunteerDto? ResponsibleVolunteer { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ChangedDate { get; set; }
    public LookupDto Sex { get; set; } = null!;
    public LookupDto? Space { get; set; }
    public LookupDto? Collar { get; set; }
    public List<NotionFile>? Images { get; set; }
    public List<LookupDto>? Tags { get; set; }
}
