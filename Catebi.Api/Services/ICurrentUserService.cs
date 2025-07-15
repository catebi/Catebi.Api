using Catebi.Api.Models.Telegram;

namespace Catebi.Api.Services;

/// <summary>
/// Service for accessing the current authenticated Telegram user
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the current authenticated user. Returns null if not authenticated.
    /// </summary>
    Task<CurrentTelegramUser?> GetCurrentUserAsync();

    /// <summary>
    /// Gets the current authenticated user. Throws exception if not authenticated.
    /// </summary>
    Task<CurrentTelegramUser> GetRequiredCurrentUserAsync();

    /// <summary>
    /// Gets the Telegram ID of the current user. Returns null if not authenticated.
    /// </summary>
    long? GetCurrentTelegramId();

    /// <summary>
    /// Checks if the current user is authenticated
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Checks if the current user is an admin
    /// </summary>
    bool IsAdmin { get; }
} 