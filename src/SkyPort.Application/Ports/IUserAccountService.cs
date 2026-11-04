using SkyPort.Application.Common;
using SkyPort.Application.UseCases.Usuarios.Dtos;

namespace SkyPort.Application.Ports;

public interface IUserAccountService
{
    Task<IReadOnlyList<UserDto>> ListAsync(CancellationToken cancellationToken = default);
    Task<Result<UserDto>> GetAsync(string userId, CancellationToken cancellationToken = default);
    Task<Result<string>> CreateAsync(string email, string nombreCompleto, string password, string role, CancellationToken cancellationToken = default);
    Task<Result> ChangeRoleAsync(string userId, string role, CancellationToken cancellationToken = default);
    Task<Result> SetActiveAsync(string userId, bool active, CancellationToken cancellationToken = default);
}
