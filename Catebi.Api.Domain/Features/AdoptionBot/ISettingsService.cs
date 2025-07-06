using Catebi.Api.Domain.Features.AdoptionBot.Enums;

namespace Catebi.Api.Domain.Features.AdoptionBot;

public interface ISettingsService
{
    /// <summary>
    /// Get a setting value by key (uses Value or TestValue based on environment)
    /// </summary>
    Task<long> GetSettingValue(SettingValues key);

    /// <summary>
    /// Get work chat ID for notifications
    /// </summary>
    Task<long> GetWorkChatId();

    /// <summary>
    /// Get event topic ID for work chat notifications
    /// </summary>
    Task<long> GetEventTopicId();
} 