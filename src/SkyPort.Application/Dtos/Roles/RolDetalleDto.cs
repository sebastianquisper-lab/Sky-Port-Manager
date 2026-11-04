namespace SkyPort.Application.Dtos.Roles;

public sealed record RolDetalleDto(string Id, string Name, IReadOnlyList<string> UserEmails);
