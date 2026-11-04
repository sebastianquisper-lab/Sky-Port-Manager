using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Ports;

namespace SkyPort.Application.UseCases.Roles.Commands;

public sealed record UpdateRolCommand(string Id, string Name) : IRequest<Result>;

public sealed class UpdateRolCommandHandler(IRolIdentityService roles)
    : IRequestHandler<UpdateRolCommand, Result>
{
    public Task<Result> Handle(UpdateRolCommand request, CancellationToken cancellationToken) =>
        roles.RenameAsync(request.Id, request.Name.Trim(), cancellationToken);
}
