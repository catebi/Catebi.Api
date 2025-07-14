using Microsoft.AspNetCore.Authorization;
using Catebi.Api.Domain.Features.AdoptionBot;

namespace Catebi.Api.Authorization.Requirements;

/// <summary>
/// Requirement that checks if a user is registered and active in the system
/// </summary>
public class RegisteredUserRequirement : IAuthorizationRequirement { }

/// <summary>
/// Handler for RegisteredUserRequirement
/// </summary>
public class RegisteredUserHandler(IServiceProvider serviceProvider, ILogger<RegisteredUserHandler> logger) : AuthorizationHandler<RegisteredUserRequirement>
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<RegisteredUserHandler> _logger = logger;

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RegisteredUserRequirement requirement)
    {
        var telegramIdClaim = context.User.FindFirst("TelegramId");
        if (telegramIdClaim == null)
        {
            _logger.LogWarning("No TelegramId claim found");
            context.Fail();
            return;
        }

        if (!long.TryParse(telegramIdClaim.Value, out var telegramId))
        {
            _logger.LogWarning("Invalid TelegramId format: {TelegramId}", telegramIdClaim.Value);
            context.Fail();
            return;
        }

        // Check if we already have the user status in claims (from middleware)
        var userStatusClaim = context.User.FindFirst("UserStatus");
        if (userStatusClaim != null)
        {
            if (userStatusClaim.Value == "Active")
            {
                context.Succeed(requirement);
                return;
            }
            else
            {
                _logger.LogWarning("User {TelegramId} is not active. Status: {Status}",
                    telegramId, userStatusClaim.Value);
                context.Fail();
                return;
            }
        }

        // If not in claims, look up the user
        using var scope = _serviceProvider.CreateScope();
        var userService = scope.ServiceProvider.GetRequiredService<IAdoptionBotUserService>();

        try
        {
            var user = await userService.FindUserByTelegramId(telegramId);

            if (user != null && user.Status == "Active")
            {
                context.Succeed(requirement);
            }
            else
            {
                _logger.LogWarning("User {TelegramId} is not registered or not active", telegramId);
                context.Fail();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking user registration status");
            context.Fail();
        }
    }
}
