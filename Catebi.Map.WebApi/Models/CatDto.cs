public class CatDto
{
    public string CatId { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? GeoLocation { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime LastEditedTime { get; set; }
    public string Url { get; set; }
    public List<NotionFile> Images { get; set; }
}
