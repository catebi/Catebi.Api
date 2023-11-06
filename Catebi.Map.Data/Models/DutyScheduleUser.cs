namespace Catebi.Map.Data.Models;

public class DutyScheduleUser(string name, string telegramAccount)
{
    public string Name { get; set; } = name;
    public string TelegramAccount { get; set; } = telegramAccount;
}
