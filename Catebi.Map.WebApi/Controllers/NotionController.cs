using Catebi.Map.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Notion.Client;

namespace Catebi.Map.WebApi.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class NotionController : ControllerBase
{
    private readonly ILogger<NotionController> _logger;
    private readonly INotionClient _client;
    private readonly NotionApiSettings _notionSettings;

    public NotionController(
        INotionClient client,
        IOptions<NotionApiSettings> notionSettings,
        ILogger<NotionController> logger)
    {
        _client = client;
        _notionSettings = notionSettings.Value;
        _logger = logger;
    }

    public async Task<PaginatedList<Page>> GetCats()
    {
        var dateFilter = new DateFilter("When", onOrAfter: DateTime.Now);
        var queryParams = new DatabasesQueryParameters();
        //{ Filter = dateFilter };
        var pages = await _client.Databases.QueryAsync(_notionSettings.DatabaseIds[NotionDb.Cats], queryParams);
        return pages;
    }
}
