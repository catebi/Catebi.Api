using Microsoft.AspNetCore.Mvc;

namespace Catebi.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class NotionSyncController : ControllerBase
{
    private readonly INotionApiService _notionService;
    private readonly ILogger<NotionSyncController> _logger;

    public NotionSyncController(
        INotionApiService notionService,
        ILogger<NotionSyncController> logger)
    {
        _notionService = notionService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<bool> SyncDicts() => await _notionService.SyncDicts();

    [HttpGet]
    public async Task<bool> SyncVolunteers() => await _notionService.SyncVolunteers();

    [HttpGet]
    public async Task<bool> SyncCats() => await _notionService.SyncCats();
}
