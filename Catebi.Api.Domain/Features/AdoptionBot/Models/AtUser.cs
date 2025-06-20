using Catebi.Api.Domain.Features.AdoptionBot.Enums;
using System.Text.Json.Serialization;

namespace Catebi.Api.Domain.Features.AdoptionBot.Models;

public class AtUser
{
    public string? RecordId { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; }
    public string Telegram { get; set; }
    public long TelegramChatId { get; set; }
    public bool UsePayedAccount { get; set; }
    public bool IsVolunteer { get; set; }

    [JsonPropertyName("Status")]
    public string StatusValue { get; set; }

    [JsonPropertyName("Role")]
    public string RoleValue { get; set; }

    [JsonIgnore]
    public UserStatuses Status => Enum.Parse<UserStatuses>(StatusValue);

    [JsonIgnore]
    public UserRoles Role => Enum.Parse<UserRoles>(RoleValue);
}
