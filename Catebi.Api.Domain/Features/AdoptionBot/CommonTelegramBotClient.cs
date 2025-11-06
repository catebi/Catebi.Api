using Telegram.Bot;

namespace Catebi.Api.Domain.Features.AdoptionBot;

/// <summary>
/// Wrapper for the common Telegram bot client used for work chat notifications
/// </summary>
public class CommonTelegramBotClient(TelegramBotClient client)
{
    public TelegramBotClient Client { get; } = client;
}
