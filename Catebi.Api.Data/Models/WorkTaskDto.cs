namespace Catebi.Api.Data.Models;

public class WorkTaskDto
{
    public int Id { get; set; }
    public int TelegramThreadId { get; set; }
    public string Description { get; set; } = null!;
    public string Status { get; set; } = null!;
    public int StatusCode { get; set; }
    public string? ResponsibleUserTg { get; set; }
    public string CreatedByTg { get; set; }
    public string ChangedByTg { get; set; }
    public DateTime? ReminderDate { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? ChangedDate { get; set; }
}