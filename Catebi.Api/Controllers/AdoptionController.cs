using Catebi.Api.Domain.Features.AdoptionBot;
using Microsoft.AspNetCore.Mvc;

namespace Catebi.Api.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AdoptionController( IAdoptionBotActionService adoptionService,
                                 ILogger<AdoptionController> logger) : ControllerBase
{
    private readonly ILogger<AdoptionController> _logger = logger;
    private readonly IAdoptionBotActionService _adoptionService = adoptionService;

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

    [HttpGet]
    public async Task<IActionResult> ConfirmCatPayment([FromQuery] string id)
    {
        try
        {
            var result = await _adoptionService.ConfirmCatPayment(id);
            if (result)
            {
                return Ok(new { Message = $"Cat payment ({id}) confirmed and notified." });
            }

            return StatusCode(500, new { Message = $"Failed to confirm cat payment {id}." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming cat payment");
            return StatusCode(500, new { Message = ex.Message });
        }
    }
}
