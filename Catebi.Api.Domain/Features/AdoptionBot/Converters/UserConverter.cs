namespace Catebi.Api.Domain.Features.AdoptionBot.Converters;

public static class UserConverter
{
    public static UserDto ToDto(AtUser user) => new()
    {
        RecordId = user.RecordId,
        UserId = user.UserId,
        Name = user.Name,
        Telegram = user.Telegram,
        TelegramChatId = user.TelegramChatId,
        UsePayedAccount = user.UsePayedAccount,
        IsVolunteer = user.IsVolunteer,
        Status = user.StatusValue,
        Role = user.RoleValue,
        Language = user.LanguageValue,
        AdditionalContact = user.AdditionalContact,
        Notes = user.Notes
    };

    public static AtUser ToAtUser(UserDto dto) => new()
    {
        RecordId = dto.RecordId,
        UserId = dto.UserId ?? 0,
        Name = dto.Name,
        Telegram = dto.Telegram,
        TelegramChatId = dto.TelegramChatId,
        UsePayedAccount = dto.UsePayedAccount ?? false,
        IsVolunteer = dto.IsVolunteer ?? false,
        StatusValue = dto.Status ?? string.Empty,
        RoleValue = dto.Role ?? string.Empty,
        LanguageValue = dto.Language ?? string.Empty,
        AdditionalContact = dto.AdditionalContact,
        Notes = dto.Notes
    };
}
