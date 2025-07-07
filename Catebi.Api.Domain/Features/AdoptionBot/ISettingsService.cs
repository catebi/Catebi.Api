using Catebi.Api.Domain.Features.AdoptionBot.Enums;

namespace Catebi.Api.Domain.Features.AdoptionBot;

public interface ISettingsService
{
    /// <summary>
    /// Get a setting value by key (uses Value or TestValue based on environment)
    /// </summary>
    Task<long> GetSettingValue(SettingValues key);

    /// <summary>
    /// Get the chat ID and topic ID for the adoption bot
    /// </summary>
    /// <returns>Tuple containing chat ID and topic ID</returns>
    Task<(long ChatId, long TopicId)> GetChatTopicInfo();
}
