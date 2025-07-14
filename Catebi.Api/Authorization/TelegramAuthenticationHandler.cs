using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Catebi.Api.Authorization;

public class TelegramAuthenticationSchemeOptions : AuthenticationSchemeOptions { }

public class TelegramAuthenticationHandler(
    IOptionsMonitor<TelegramAuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder) : AuthenticationHandler<TelegramAuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // The actual authentication is done by the middleware
        // This handler just needs to recognize if a user was authenticated by the middleware

        if (Context.User.Identity?.IsAuthenticated == true &&
            Context.User.Identity.AuthenticationType == "Telegram")
        {
            // User was authenticated by our middleware
            var ticket = new AuthenticationTicket(Context.User, Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        // No authentication was performed
        return Task.FromResult(AuthenticateResult.NoResult());
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        // Return 401 Unauthorized with a specific message for Telegram auth
        Response.StatusCode = 401;
        Response.Headers["WWW-Authenticate"] = "Telegram";
        return Response.WriteAsync("Telegram authentication required. Please provide valid initData in the Authorization header.");
    }
}
