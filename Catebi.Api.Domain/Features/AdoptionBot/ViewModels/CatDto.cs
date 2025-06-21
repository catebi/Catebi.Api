namespace Catebi.Api.Domain.Features.AdoptionBot.ViewModels;

public class CatDto
{
    public int? CatId { get; set; }
    public string? RecordId { get; set; }
    public string OwnerRecordId { get; set; }
    public string Name { get; set; }
    public string DateOfBirth { get; set; }
    public List<AttachmentDto>? Photos { get; set; }
    public AttachmentDto? MainPhoto { get; set; }
    public string Status { get; set; }
}
