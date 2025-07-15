using Microsoft.AspNetCore.Authorization;
using Catebi.Api.Extensions;

namespace Catebi.Api.Authorization.Requirements;

/// <summary>
/// Requirement that checks if a user has Admin role
/// </summary>
public class AdminRequirement : IAuthorizationRequirement { }

public class AdminHandler(ILogger<AdminHandler> logger) : AuthorizationHandler<AdminRequirement>
{
    private readonly ILogger<AdminHandler> _logger = logger;

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AdminRequirement requirement)
    {
        // Check if user has Admin role using extension method
        if (context.User.IsAdmin())
        {
            _logger.LogInformation("User has Admin role");
            context.Succeed(requirement);
        }
        else
        {
            _logger.LogWarning("User does not have Admin role");
            context.Fail();
        }

        return Task.CompletedTask;
    }
}
