using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Ports;

namespace SkyPort.Application.UseCases.Usuarios.Commands;

public sealed record SetUserActiveCommand(string UserId, bool Active) : IRequest<Result>;

public sealed class SetUserActiveCommandHandler(IUserAccountService userAccountService)
    : IRequestHandler<SetUserActiveCommand, Result>
{
    public Task<Result> Handle(SetUserActiveCommand request, CancellationToken cancellationToken) =>
        userAccountService.SetActiveAsync(request.UserId, request.Active, cancellationToken);
}
