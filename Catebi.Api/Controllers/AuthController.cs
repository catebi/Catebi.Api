using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Catebi.Api.Controllers;

[Route("api/[controller]/[action]")]
public class AuthController : ControllerBase
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<AuthController> _logger;
    private readonly IEmailSender _emailSender;

    public AuthController(
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager,
        IEmailSender emailSender,
        ILogger<AuthController> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
        _emailSender = emailSender;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var user = new IdentityUser { UserName = model.Email, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Action(
                nameof(ConfirmEmail),
                "Auth",
                new { userId = user.Id, code = code },
                protocol: HttpContext.Request.Scheme,
                host: HttpContext.Request.Host.Value);

            await _emailSender.SendEmailAsync(model.Email, "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            return Ok(new { Message = "Registration successful, please check your email to confirm your account." });
        }
        else
        {
            return BadRequest(result.Errors);
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail(string userId, string code)
    {
        if (userId == null || code == null)
        {
            return BadRequest("Invalid user data");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound("User not found");
        }

        var result = await _userManager.ConfirmEmailAsync(user, code);
        if (result.Succeeded)
        {
            return Ok(new { Message = "Email confirmed successfully" });
        }
        else
        {
            return BadRequest("Error confirming your email");
        }
    }

    [HttpPost("logout")]
    [Authorize] // Ensure this endpoint is protected
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("User logged out.");
        return Ok(new { message = "You have been logged out successfully." });
    }
}