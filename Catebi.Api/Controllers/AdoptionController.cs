using Microsoft.AspNetCore.Mvc;

namespace Catebi.Api.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AdoptionController( IAdoptionService adoptionService,
                                 ILogger<AdoptionController> logger) : ControllerBase
{
    private readonly ILogger<AdoptionController> _logger = logger;
    private readonly IAdoptionService _adoptionService = adoptionService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CatDto>>> Test()
    {
        var users = await _adoptionService.GetUserRecords();
        return Ok(users);
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmUser([FromQuery] string id)
    {
        try
        {
            var result = await _adoptionService.ConfirmUser(id);
            if (result)
            {
                return Ok(new { Message = $"User {id} confirmed and notified." });
            }

            return StatusCode(500, new { Message = $"Failed to confirm user {id}." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming user");
            return StatusCode(500, new { Message = ex.Message });
        }
    }
}
