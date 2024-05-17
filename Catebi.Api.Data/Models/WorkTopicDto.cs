namespace Catebi.Api.Data.Models;

public class WorkTopicDto
{
    public int Id { get; set; }
    public int TelegramThreadId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsMain { get; set; }
}