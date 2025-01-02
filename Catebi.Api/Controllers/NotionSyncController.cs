using Microsoft.AspNetCore.Mvc;

namespace Catebi.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class NotionSyncController(
    INotionApiService notionService,
    ILogger<NotionSyncController> logger) : ControllerBase
{
    private readonly INotionApiService _notionService = notionService;
    private readonly ILogger<NotionSyncController> _logger = logger;

    [HttpGet]
    public async Task<bool> SyncDicts() => await _notionService.SyncDicts();

    [HttpGet]
    public async Task<bool> SyncVolunteers() => await _notionService.SyncVolunteers();

    [HttpGet]
    public async Task<bool> SyncCats() => await _notionService.SyncCats();
}
