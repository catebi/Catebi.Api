namespace Catebi.Api.Data.Models;

public class UpdateWorkTaskDto
{
    public string Message { get; set; } = null!;
    public string UserTg { get; set; } = null!;
    public int? TopicId { get; set; }
}