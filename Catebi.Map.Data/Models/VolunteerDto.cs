namespace Catebi.Map.Data.Models;

public class VolunteerDto
{
    public int Id { get; set; }
    public string? NotionVolunteerId { get; set; }
    public string Name { get; set; } = null!;
    public string? NotionUser { get; set; }
    public string? TelegramAccount { get; set; }
    public string? Address { get; set; }
    public string? GeoLocation { get; set; }
}