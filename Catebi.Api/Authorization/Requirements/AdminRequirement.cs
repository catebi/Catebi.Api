using Microsoft.AspNetCore.Authorization;

namespace Catebi.Api.Authorization.Requirements;

/// <summary>
/// Requirement that checks if a user has Admin role
/// </summary>
public class AdminRequirement : IAuthorizationRequirement { }

/// <summary>
/// Handler for AdminRequirement
/// </summary>
public class AdminHandler(ILogger<AdminHandler> logger) : AuthorizationHandler<AdminRequirement>
{
    private readonly ILogger<AdminHandler> _logger = logger;

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AdminRequirement requirement)
    {
        // Check if user has Admin role claim
        if (context.User.IsInRole("Admin"))
        {
            _logger.LogInformation("User has Admin role");
            context.Succeed(requirement);
        }
        else
        {
            // Also check the UserRole claim
            var userRoleClaim = context.User.FindFirst("UserRole");
            if (userRoleClaim != null && userRoleClaim.Value == "Admin")
            {
                _logger.LogInformation("User has Admin role from UserRole claim");
                context.Succeed(requirement);
            }
            else
            {
                _logger.LogWarning("User does not have Admin role");
                context.Fail();
            }
        }

        return Task.CompletedTask;
    }
}
