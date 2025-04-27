namespace Catebi.Api.Domain.Models;

public class FileStorageDto
{
    public Guid FileStorageId { get; set; }
    public string FileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long Size { get; set; }
    public DateTime Created { get; set; }
    public byte[] Data { get; set; } = null!;
}
