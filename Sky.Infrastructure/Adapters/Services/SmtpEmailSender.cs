using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sky.Application.Ports;

namespace Sky.Infrastructure.Adapters.Services;

public sealed class SmtpEmailSender(IConfiguration config, ILogger<SmtpEmailSender> logger) : IEmailSender
{
    public async Task SendAsync(
        string to,
        string subject,
        string htmlBody,
        IReadOnlyCollection<(string FileName, byte[] Content, string ContentType)>? attachments = null,
        CancellationToken ct = default)
    {
        var host = config["Smtp:Host"];

        if (string.IsNullOrWhiteSpace(host))
        {
            logger.LogWarning("SMTP sin configurar: no se envió el correo '{Subject}' a {To}.", subject, to);
            return;
        }

        using var message = new MailMessage
        {
            From = new MailAddress(config["Smtp:FromAddress"] ?? "no-reply@skyport.local", config["Smtp:FromName"] ?? "SkyPort Manager"),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };
        message.To.Add(to);

        // Attachment es dueño del stream y lo libera junto con el mensaje.
        foreach (var (fileName, content, contentType) in attachments ?? [])
        {
            message.Attachments.Add(new Attachment(new MemoryStream(content), fileName, contentType));
        }

        using var client = new SmtpClient(host, config.GetValue("Smtp:Port", 587)) { EnableSsl = config.GetValue("Smtp:EnableSsl", true) };
        var user = config["Smtp:User"];
        if (!string.IsNullOrWhiteSpace(user))
        {
            client.Credentials = new NetworkCredential(user, config["Smtp:Password"]);
        }

        await client.SendMailAsync(message, ct);
    }
}
