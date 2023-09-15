namespace Catebi.Map.WebApi.Models;

public class NotionApiSettings
{
    public string AuthToken { get; set; }
    public Dictionary<NotionDb, string> DatabaseIds { get; set; }
}