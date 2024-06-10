namespace Catebi.Api.Data.Models;

public class CreateWorkTopicDto
{
    public int TelegramThreadId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string CreatedBy { get; set; } = null!;
}