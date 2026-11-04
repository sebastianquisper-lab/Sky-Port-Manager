namespace SkyPort.Application.Dtos.Usuarios;

public sealed record UsuarioDetalleDto(string Id, string? Email, string? UserName, bool EmailConfirmado, IReadOnlyList<string> Roles);
