using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Ports;

namespace SkyPort.Application.UseCases.Usuarios.Commands;

public sealed record ChangeUserRoleCommand(string UserId, string Role) : IRequest<Result>;

public sealed class ChangeUserRoleCommandHandler(IUserAccountService userAccountService)
    : IRequestHandler<ChangeUserRoleCommand, Result>
{
    public Task<Result> Handle(ChangeUserRoleCommand request, CancellationToken cancellationToken) =>
        userAccountService.ChangeRoleAsync(request.UserId, request.Role, cancellationToken);
}
