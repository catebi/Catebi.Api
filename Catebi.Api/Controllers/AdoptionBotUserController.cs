using Catebi.Api.Domain.Features.AdoptionBot;
using Catebi.Api.Domain.Features.AdoptionBot.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Catebi.Api.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class AdoptionBotUserController( IAdoptionBotUserService             UserService ,
                                        ILogger<AdoptionBotUserController>  Logger        ) : ControllerBase
{
    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> RegisterUser([FromBody] UserDto user)
    {
        try
        {
            var result = await UserService.RegisterUser(user);
            return Ok(result);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "⚠️ Error registering user");
            return StatusCode(500, new { ex.Message });
        }
    }

    /// <summary>
    /// Find user by Telegram ID
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> FindUserByTelegramId([FromQuery] long telegramId)
    {
        try
        {
            var result = await UserService.FindUserByTelegramId(telegramId);
            if (result == null)
            {
                return NotFound(new { Message = $"User with Telegram ID {telegramId} not found." });
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "⚠️ Error finding user by Telegram ID");
            return StatusCode(500, new { ex.Message });
        }
    }

    /// <summary>
    /// Update user data
    /// </summary>
    [HttpPut]
    public async Task<IActionResult> UpdateUser([FromBody] UserDto user)
    {
        try
        {
            if (string.IsNullOrEmpty(user.RecordId))
            {
                return BadRequest(new { Message = "Record ID is required for updating a user." });
            }

            var result = await UserService.UpdateUser(user);
            return Ok(result);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "⚠️ Error updating user");
            return StatusCode(500, new { ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetUserPayments([FromQuery] string userId)
    {
        try
        {
            var payments = await UserService.GetUserPayments(userId);
            return Ok(payments);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "⚠️ Error getting user payments");
            return StatusCode(500, new { ex.Message });
        }
    }

    /// <summary>
    /// Get all cats for a user
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetCats([FromQuery] string userId)
    {
        try
        {
            var cats = await UserService.GetCats(userId);
            return Ok(cats);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "⚠️ Error getting cats for user");
            return StatusCode(500, new { ex.Message });
        }
    }
}
