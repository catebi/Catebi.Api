namespace Catebi.Api.Domain.Features.AdoptionBot.Converters;

public static class UserConverter
{
    public static UserDto ToDto(AtUser user) => new()
    {
            UserId = user.UserId,
            RecordId = user.RecordId,
            Name = user.Name,
            Telegram = user.Telegram,
            TelegramChatId = user.TelegramChatId,
            UsePayedAccount = user.UsePayedAccount,
            IsVolunteer = user.IsVolunteer,
            Status = user.StatusValue,
            Role = user.RoleValue
    };
} 