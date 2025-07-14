using Catebi.Api.Domain.Features.AdoptionBot;
using Catebi.Api.Domain.Features.AdoptionBot.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Catebi.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Catebi.Api.Controllers;

[Route("[controller]/[action]")]
[ApiController]
[TelegramAuthorize] // Requires authenticated Telegram user
public class AdoptionBotUserController(IAdoptionBotUserService UserService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> RegisterUser([FromBody] UserDto user)
    {
        // Get current user's Telegram ID from claims
        var currentTelegramId = User.FindFirst("TelegramId")?.Value;

        // Verify they're registering themselves
        if (currentTelegramId != null && user.TelegramChatId.ToString() != currentTelegramId)
        {
            return Forbid("You can only register yourself");
        }

        var result = await UserService.RegisterUser(user);
        return Ok(result);
    }

    [HttpGet]
    [AllowAnonymous] // Allow checking user status without authentication
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

        // Get current user's info from claims
        var currentTelegramId = User.FindFirst("TelegramId")?.Value;
        var currentUserRecordId = User.FindFirst("UserRecordId")?.Value;
        var isAdmin = User.IsInRole("Admin");

        // Verify they're updating their own record (unless admin)
        if (!isAdmin && currentUserRecordId != user.RecordId)
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
        // Get current user's info from claims
        var currentUserRecordId = User.FindFirst("UserRecordId")?.Value;
        var isAdmin = User.IsInRole("Admin");

        // Verify they're accessing their own payments (unless admin)
        if (!isAdmin && currentUserRecordId != userRecordId)
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
        // Get current user's info from claims
        var currentUserRecordId = User.FindFirst("UserRecordId")?.Value;
        var isAdmin = User.IsInRole("Admin");

        // Verify they're accessing their own cats (unless admin)
        if (!isAdmin && currentUserRecordId != userRecordId)
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
        // Get current user's Telegram ID from claims
        var telegramIdClaim = User.FindFirst("TelegramId")?.Value;

        if (string.IsNullOrEmpty(telegramIdClaim) || !long.TryParse(telegramIdClaim, out var telegramId))
        {
            return BadRequest(new { Message = "Invalid user context" });
        }

        var user = await UserService.FindUserByTelegramId(telegramId);
        if (user == null)
        {
            return NotFound(new { Message = "Current user not found" });
        }

        // Add additional info from claims
        var userInfo = new
        {
            user.RecordId,
            user.Name,
            user.Telegram,
            user.TelegramChatId,
            user.Status,
            user.Role,
            user.Language,
            user.IsVolunteer,
            user.UsePayedAccount,
            user.AdditionalContact,
            user.Notes,
            // Additional info from claims
            IsPremium = User.FindFirst("IsPremium")?.Value == "True",
            FirstName = User.FindFirst("FirstName")?.Value,
            LastName = User.FindFirst("LastName")?.Value,
            Username = User.FindFirst("Username")?.Value
        };

        return Ok(userInfo);
    }
}
