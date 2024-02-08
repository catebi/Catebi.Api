using Microsoft.AspNetCore.Mvc;

namespace Catebi.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class FreeganController : ControllerBase
{
    private readonly IFreeganMessageService _freeganService;

    public FreeganController(IFreeganMessageService freeganService)
    {
        _freeganService = freeganService;
    }

    [HttpPost]
    public async Task<bool> SaveMessage(FreeganMessageDto data) => await _freeganService.SaveMessage(data);
}
