using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using Remp.Models.Settings;
using Remp.Service.Interfaces;

namespace Remp.Service.Services;

public class EmailService : IEmailService
{
    private readonly EmailSetting _emailSetting;

    public EmailService(IOptions<EmailSetting> emailSetting)
    {
        _emailSetting = emailSetting.Value;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_emailSetting.SenderEmail, _emailSetting.SenderEmail));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;
        
        var builder = new BodyBuilder { HtmlBody = body };
        message.Body = builder.ToMessageBody();

        using var client = new SmtpClient();

        try
        {
            await client.ConnectAsync(
                _emailSetting.SmtpServer,
                _emailSetting.SmtpPort,
                _emailSetting.SmtpPort == 587
                ? MailKit.Security.SecureSocketOptions.StartTls
                : MailKit.Security.SecureSocketOptions.SslOnConnect
                );

            await client.AuthenticateAsync(_emailSetting.SenderEmail, _emailSetting.Password);
            await client.SendAsync(message);
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            await client.DisconnectAsync(true);
        }
    }
}
