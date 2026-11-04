using SkyPort.Application.Dtos.Usuarios;

namespace SkyPort.Web.Models;

public sealed record UsuarioAdminDetailsViewModel(
    UsuarioDetalleDto Usuario,
    IReadOnlyList<UsuarioRolMembershipDto> Roles);
