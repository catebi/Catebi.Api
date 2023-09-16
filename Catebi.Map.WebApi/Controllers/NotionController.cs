using Microsoft.AspNetCore.Mvc;

namespace Catebi.Map.WebApi.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class NotionController : ControllerBase
{
    private readonly INotionApiService _notionService;
    private readonly ILogger<NotionController> _logger;

    public NotionController(
        INotionApiService notionService,
        ILogger<NotionController> logger)
    {
        _notionService = notionService;
        _logger = logger;
    }

    public async Task<List<CatDto>> GetCats() => await _notionService.GetCats();
}
