using Catebi.Api.Domain.Features.AdoptionBot;
using Catebi.Api.Domain.Features.AdoptionBot.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Catebi.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Catebi.Api.Services;

namespace Catebi.Api.Controllers;

[Route("[controller]/[action]")]
[ApiController]
[TelegramAuthorize]
public class AdoptionBotUserController(
    IAdoptionBotUserService UserService,
    ICurrentUserService CurrentUserService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> RegisterUser([FromBody] UserDto user)
    {
        var currentTelegramId = CurrentUserService.GetCurrentTelegramId();

        // Verify they're registering themselves
        if (user.TelegramChatId != currentTelegramId)
        {
            return Forbid("You can only register yourself");
        }

        var result = await UserService.RegisterUser(user);
        return Ok(result);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> FindUserByTelegramId([FromQuery] long telegramId)
    {
        var result = await UserService.FindUserByTelegramId(telegramId);
        if (result == null)
        {
            return NotFound(new { Message = $"User with Telegram ID {telegramId} not found." });
        }
        return Ok(result);
    }

    [HttpPut]
    [Authorize(Policy = "RegisteredUser")]
    public async Task<IActionResult> UpdateUser([FromBody] UserDto user)
    {
        if (string.IsNullOrEmpty(user.RecordId))
        {
            return BadRequest(new { Message = "Record ID is required for updating a user." });
        }

        var currentUser = await CurrentUserService.GetRequiredCurrentUserAsync();

        // Verify they're updating their own record (unless admin)
        if (!currentUser.IsAdmin && currentUser.RecordId != user.RecordId)
        {
            return Forbid("You can only update your own information");
        }

        var result = await UserService.UpdateUser(user);
        return Ok(result);
    }

    [HttpGet]
    [Authorize(Policy = "RegisteredUser")]
    public async Task<IActionResult> GetUserPayments([FromQuery] string userRecordId)
    {
        var currentUser = await CurrentUserService.GetRequiredCurrentUserAsync();

        // Verify they're accessing their own payments (unless admin)
        if (!currentUser.IsAdmin && currentUser.RecordId != userRecordId)
        {
            return Forbid("You can only view your own payments");
        }

        var payments = await UserService.GetUserPayments(userRecordId);
        return Ok(payments);
    }

    [HttpGet]
    [Authorize(Policy = "RegisteredUser")]
    public async Task<IActionResult> GetCats([FromQuery] string userRecordId)
    {
        var currentUser = await CurrentUserService.GetRequiredCurrentUserAsync();

        // Verify they're accessing their own cats (unless admin)
        if (!currentUser.IsAdmin && currentUser.RecordId != userRecordId)
        {
            return Forbid("You can only view your own cats");
        }

        var cats = await UserService.GetCats(userRecordId);
        return Ok(cats);
    }

    [HttpGet]
    [Authorize(Policy = "RegisteredUser")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var currentUser = await CurrentUserService.GetRequiredCurrentUserAsync();

        // Return comprehensive user information
        var userInfo = new
        {
            // Database user info
            currentUser.RecordId,
            currentUser.Name,
            currentUser.Telegram,
            TelegramChatId = currentUser.TelegramId, // Keep compatibility with existing API
            currentUser.Status,
            Role = currentUser.Role?.ToString(),
            currentUser.Language,
            currentUser.IsVolunteer,
            currentUser.UsePayedAccount,
            currentUser.AdditionalContact,
            currentUser.Notes,

            // Telegram user info
            currentUser.IsPremium,
            currentUser.FirstName,
            currentUser.LastName,
            currentUser.Username,
            currentUser.LanguageCode,
            currentUser.PhotoUrl,
            currentUser.AllowsWriteToPm,

            // Computed properties
            currentUser.DisplayName,
            currentUser.FullName,
            currentUser.IsAuthenticated,
            currentUser.IsAdmin,
            currentUser.IsRegistered
        };

        return Ok(userInfo);
    }
}
