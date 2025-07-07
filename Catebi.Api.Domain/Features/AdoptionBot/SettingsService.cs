using Catebi.Api.Domain.Features.AdoptionBot.Enums;
using Catebi.Api.Domain.Features.AdoptionBot.Models;
using Microsoft.Extensions.Hosting;

namespace Catebi.Api.Domain.Features.AdoptionBot;

public class SettingsService(
    IAirtableRepository AirtableRepository,
    IHostEnvironment Environment,
    ILogger<SettingsService> Logger) : ISettingsService
{
    private readonly string SettingsTableName = AirTables.Settings.ToString();
    private readonly Dictionary<string, AtSettings> _settingsCache = new();
    private DateTime _lastCacheUpdate = DateTime.MinValue;
    private readonly TimeSpan _cacheValidityPeriod = TimeSpan.FromMinutes(100);

    public async Task<long> GetSettingValue(SettingValues key)
    {
        var keyString = key.ToString();
        Logger.LogInformation($"Getting setting value for key: {keyString}");

        await EnsureSettingsLoaded();

        if (!_settingsCache.TryGetValue(keyString, out var setting))
        {
            throw new Exception($"Setting '{keyString}' not found in Settings table");
        }

        // Use TestValue in Development environment, Value in Production
        var isDevelopment = Environment.IsDevelopment();
        var value = isDevelopment ? setting.TestValue : setting.Value;

        Logger.LogInformation($"Retrieved setting {keyString}: {value} (Environment: {(isDevelopment ? "Development" : "Production")})");
        return value;
    }

    public async Task<(long ChatId, long TopicId)> GetChatTopicInfo()
    {
        var workChatId = await GetSettingValue(SettingValues.WorkChatId);
        var eventTopicId = await GetSettingValue(SettingValues.EventTopicId);
        return (Convert.ToInt64($"-100{workChatId}"), eventTopicId);
    }

    private async Task EnsureSettingsLoaded()
    {
        // Check if cache is still valid
        if (_settingsCache.Count > 0 && DateTime.UtcNow - _lastCacheUpdate < _cacheValidityPeriod)
        {
            return;
        }

        Logger.LogInformation("Loading settings from Airtable...");

        try
        {
            var response = await AirtableRepository.ListRecords<AtSettings>(SettingsTableName, filterByFormula: null);

            if (!response.Success)
            {
                Logger.LogError($"Error loading settings: {response.AirtableApiError.ErrorMessage}");
                throw new Exception($"Error loading settings: {response.AirtableApiError.ErrorMessage}");
            }

            _settingsCache.Clear();
            foreach (var record in response.Records)
            {
                _settingsCache[record.Fields.Key] = record.Fields;
            }

            _lastCacheUpdate = DateTime.UtcNow;
            Logger.LogInformation($"Loaded {_settingsCache.Count} settings from Airtable");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load settings from Airtable");
            throw;
        }
    }
}
