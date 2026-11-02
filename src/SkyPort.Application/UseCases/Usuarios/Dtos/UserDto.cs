namespace SkyPort.Application.UseCases.Usuarios.Dtos;

public sealed record UserDto(string Id, string Email, string NombreCompleto, string Role, bool Activo);
