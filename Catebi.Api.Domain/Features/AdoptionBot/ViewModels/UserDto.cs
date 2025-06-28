namespace Catebi.Api.Domain.Features.AdoptionBot.ViewModels;

public class UserDto
{
    public int? UserId { get; set; }
    public string? RecordId { get; set; }
    public string Name { get; set; }
    public string Telegram { get; set; }
    public long TelegramChatId { get; set; }
    public bool? UsePayedAccount { get; set; }
    public bool? IsVolunteer { get; set; }
    public string? Status { get; set; }
    public string? Role { get; set; }
    public string? Language { get; set; }
    public string? AdditionalContact { get; set; }
    public string? Notes { get; set; }
}
