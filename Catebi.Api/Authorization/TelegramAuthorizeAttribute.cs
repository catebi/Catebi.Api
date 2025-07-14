using Microsoft.AspNetCore.Authorization;

namespace Catebi.Api.Authorization;

/// <summary>
/// Authorization attribute that requires Telegram authentication
/// </summary>
public class TelegramAuthorizeAttribute : AuthorizeAttribute
{
    public TelegramAuthorizeAttribute()
    {
        AuthenticationSchemes = "Telegram";
    }
}
