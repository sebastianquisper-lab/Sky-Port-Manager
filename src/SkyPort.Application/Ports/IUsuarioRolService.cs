using SkyPort.Application.Common;
using SkyPort.Application.Dtos.Usuarios;

namespace SkyPort.Application.Ports;

public interface IUsuarioRolService
{
    Task<IReadOnlyList<UsuarioRolMembershipDto>?> GetMembershipsAsync(string userId, CancellationToken cancellationToken);

    Task<Result> AssignAsync(string userId, string roleId, CancellationToken cancellationToken);

    Task<Result> RevokeAsync(string userId, string roleId, CancellationToken cancellationToken);
}
