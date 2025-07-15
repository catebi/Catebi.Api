using System.Security.Claims;
using Catebi.Api.Domain.Features.AdoptionBot.Enums;

namespace Catebi.Api.Extensions;

/// <summary>
/// Extension methods for working with UserRoles
/// </summary>
public static class UserRoleExtensions
{
    /// <summary>
    /// Checks if the user has the specified role
    /// </summary>
    public static bool IsInRole(this ClaimsPrincipal user, UserRoles role) => user.IsInRole(role.ToString());

    /// <summary>
    /// Gets the user's role as an enum
    /// </summary>
    public static UserRoles? GetUserRole(this ClaimsPrincipal user)
    {
        var roleString = user.FindFirst(ClaimTypes.Role)?.Value;
        if (string.IsNullOrEmpty(roleString))
            return null;

        if (Enum.TryParse<UserRoles>(roleString, ignoreCase: true, out var role))
            return role;

        return null;
    }

    /// <summary>
    /// Checks if the user is an admin
    /// </summary>
    public static bool IsAdmin(this ClaimsPrincipal user) => user.IsInRole(UserRoles.Admin);

    /// <summary>
    /// Checks if the user is a cat owner
    /// </summary>
    public static bool IsCatOwner(this ClaimsPrincipal user) => user.IsInRole(UserRoles.CatOwner);
} 