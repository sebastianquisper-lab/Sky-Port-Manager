namespace SkyPort.Application.Dtos.Usuarios;

public sealed record UsuarioDto(string Id, string? Email, string? UserName, bool EmailConfirmado);
