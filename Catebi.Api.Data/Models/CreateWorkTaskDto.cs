namespace Catebi.Api.Data.Models;

public class CreateWorkTaskDto
{
    public string Message { get; set; } = null!;
    public string UserTg { get; set; } = null!;

    public int TgThread { get; set; }
}