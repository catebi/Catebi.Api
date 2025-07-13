using Catebi.Api.Domain.Features.AdoptionBot;
using Catebi.Api.Domain.Features.AdoptionBot.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Catebi.Api.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class AdoptionBotUserController(IAdoptionBotUserService UserService) : ControllerBase
{
    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> RegisterUser([FromBody] UserDto user)
    {
        var result = await UserService.RegisterUser(user);
        return Ok(result);
    }

    /// <summary>
    /// Find user by Telegram ID
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> FindUserByTelegramId([FromQuery] long telegramId)
    {
        var result = await UserService.FindUserByTelegramId(telegramId);
        if (result == null)
        {
            return NotFound(new { Message = $"User with Telegram ID {telegramId} not found." });
        }
        return Ok(result);
    }

    /// <summary>
    /// Update user data
    /// </summary>
    [HttpPut]
    public async Task<IActionResult> UpdateUser([FromBody] UserDto user)
    {
        if (string.IsNullOrEmpty(user.RecordId))
        {
            return BadRequest(new { Message = "Record ID is required for updating a user." });
        }

        var result = await UserService.UpdateUser(user);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetUserPayments([FromQuery] string userRecordId)
    {
        var payments = await UserService.GetUserPayments(userRecordId);
        return Ok(payments);
    }

    /// <summary>
    /// Get all cats for a user
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetCats([FromQuery] string userRecordId)
    {
        var cats = await UserService.GetCats(userRecordId);
        return Ok(cats);
    }
}
