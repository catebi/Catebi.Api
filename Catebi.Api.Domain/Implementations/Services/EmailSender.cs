using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Catebi.Api.Domain.Implementations.Services;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;

    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var emailConfig = _configuration.GetSection("EmailSettings");
        var smtpClient = new MailKit.Net.Smtp.SmtpClient();

        smtpClient.Connect(emailConfig["Host"], int.Parse(emailConfig["Port"]), bool.Parse(emailConfig["UseSSL"]));
        smtpClient.Authenticate(emailConfig["Username"], emailConfig["Password"]);

        var mailMessage = new MimeMessage
        {
            Sender = new MailboxAddress(emailConfig["SenderName"], emailConfig["Sender"]),
            Subject = subject,
        };
        mailMessage.From.Add(new MailboxAddress(emailConfig["SenderName"], emailConfig["Sender"]));
        mailMessage.To.Add(new MailboxAddress("", email));

        var bodyBuilder = new BodyBuilder { HtmlBody = message };
        mailMessage.Body = bodyBuilder.ToMessageBody();

        await smtpClient.SendAsync(mailMessage);
        smtpClient.Disconnect(true);
    }
}