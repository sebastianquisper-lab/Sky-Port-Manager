namespace Sky.Application.Ports;

public interface IEmailSender
{
    /// <summary>Envía un correo HTML, con adjuntos opcionales (nombre, contenido, tipo MIME).</summary>
    Task SendAsync(
        string to,
        string subject,
        string htmlBody,
        IReadOnlyCollection<(string FileName, byte[] Content, string ContentType)>? attachments = null,
        CancellationToken ct = default);
}
