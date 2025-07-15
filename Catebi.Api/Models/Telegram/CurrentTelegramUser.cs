using Catebi.Api.Domain.Features.AdoptionBot.ViewModels;
using Catebi.Api.Domain.Features.AdoptionBot.Enums;

namespace Catebi.Api.Models.Telegram;

/// <summary>
/// Represents the current authenticated Telegram user with both Telegram data and application data
/// </summary>
public class CurrentTelegramUser
{
    // Telegram-specific properties (from initData)
    public long TelegramId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public string? Username { get; set; }
    public string? LanguageCode { get; set; }
    public bool IsPremium { get; set; }
    public string? PhotoUrl { get; set; }
    public bool AllowsWriteToPm { get; set; }

    // Application-specific properties (from database)
    public string? RecordId { get; set; }
    public string? Name { get; set; }
    public string? Telegram { get; set; }
    public string? Status { get; set; }
    public UserRoles? Role { get; set; }
    public string? Language { get; set; }
    public bool IsVolunteer { get; set; }
    public bool UsePayedAccount { get; set; }
    public string? AdditionalContact { get; set; }
    public string? Notes { get; set; }

    // Authorization helpers
    public bool IsAuthenticated => TelegramId > 0;
    public bool IsAdmin => Role == UserRoles.Admin;
    public bool IsCatOwner => Role == UserRoles.CatOwner;
    public bool IsRegistered => !string.IsNullOrEmpty(RecordId) && Status == "Active";

    // Display name helper
    public string DisplayName => !string.IsNullOrEmpty(Name) ? Name :
                                !string.IsNullOrEmpty(Username) ? $"@{Username}" :
                                FirstName;

    // Full name helper
    public string FullName => string.IsNullOrEmpty(LastName) ? FirstName : $"{FirstName} {LastName}";
}
