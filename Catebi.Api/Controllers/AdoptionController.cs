using Catebi.Api.Domain.Features.AdoptionBot;
using Microsoft.AspNetCore.Mvc;

namespace Catebi.Api.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AdoptionController( IAdoptionBotActionService   AdoptionService,
                                 ILogger<AdoptionController> Logger         ) : ControllerBase
{
    /// <summary>
    /// User account confirmation
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Obsolete("this method is not necessary anymore")]
    [HttpGet]
    public async Task<IActionResult> ConfirmUser([FromQuery] string id)
    {
        try
        {
            var result = await AdoptionService.ConfirmUser(id);
            if (result)
            {
                return Ok(new { Message = $"🎉 user {id} confirmed and notified." });
            }

            return StatusCode(500, new { Message = $"Failed to confirm user {id}." });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "⚠️ Error confirming user");
            return StatusCode(500, new { ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmCatPayment([FromQuery] string id)
    {
        try
        {
            var result = await AdoptionService.ConfirmCatPayment(id);
            if (result)
            {
                return Ok(new { Message = $"🎉 cat payment ({id}) confirmed and notified." });
            }

            return StatusCode(500, new { Message = $"Failed to confirm cat payment {id}." });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "⚠️ Error confirming cat payment");
            return StatusCode(500, new { ex.Message });
        }
    }
}
