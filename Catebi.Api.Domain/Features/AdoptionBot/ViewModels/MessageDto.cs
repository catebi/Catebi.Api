namespace Catebi.Api.Domain.Features.AdoptionBot.ViewModels;

public class MessageDto
{
    public string? RecordId { get; set; }
    public int? MessageId { get; set; }
    public string Content { get; set; }
    public string? AdminRecordId { get; set; }
    public string? Created { get; set; }
    public string? Status { get; set; }
} 