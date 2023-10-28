namespace Catebi.Map.Data.Models;

public class NotionApiSettings
{
    public string AuthToken { get; set; }
    public Dictionary<NotionDb, string> DatabaseIds { get; set; }
}