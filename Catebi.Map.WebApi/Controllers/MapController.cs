using Microsoft.AspNetCore.Mvc;

namespace Catebi.Map.WebApi.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class MapController : ControllerBase
{
    private readonly IMapService _mapService;

    public MapController(IMapService mapService)
    {
        _mapService = mapService;
    }

    [HttpGet]
    public async Task<IEnumerable<CatDto>> GetCats() => await _mapService.GetCats();
    public async Task<IEnumerable<CatDto>> GetVolunteers() => await _mapService.GetVolunteers();

}
