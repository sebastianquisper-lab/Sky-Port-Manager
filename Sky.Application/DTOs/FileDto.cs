namespace Sky.Application.DTOs;

/// <summary>Archivo generado al vuelo (PDF de boleto o de pase de abordar).</summary>
public sealed record FileDto(string FileName, string ContentType, byte[] Content);
