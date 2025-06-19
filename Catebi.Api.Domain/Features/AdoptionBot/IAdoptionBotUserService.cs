using Catebi.Api.Domain.Features.AdoptionBot.ViewModels;

namespace Catebi.Api.Domain.Features.AdoptionBot;

public interface IAdoptionBotUserService
{
    /// <summary>
    /// Register a new user
    /// </summary>
    Task<UserDto> RegisterUser(UserDto user);

    /// <summary>
    /// Find user by Telegram ID
    /// </summary>
    Task<UserDto?> FindUserByTelegramId(long telegramId);

    /// <summary>
    /// Update user data
    /// </summary>
    Task<UserDto> UpdateUser(UserDto user);

    /// <summary>
    /// Get all payments for a user.
    /// </summary>
    Task<IEnumerable<CatPaymentDto>> GetUserPayments(string userId);

    /// <summary>
    /// Get all cats for a user
    /// </summary>
    Task<IEnumerable<CatDto>> GetCats(string userId);
}
