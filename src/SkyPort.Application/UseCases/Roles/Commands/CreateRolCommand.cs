using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Dtos.Roles;
using SkyPort.Application.Ports;

namespace SkyPort.Application.UseCases.Roles.Commands;

public sealed record CreateRolCommand(string Name) : IRequest<Result<RolDto>>;

public sealed class CreateRolCommandHandler(IRolIdentityService roles)
    : IRequestHandler<CreateRolCommand, Result<RolDto>>
{
    public Task<Result<RolDto>> Handle(CreateRolCommand request, CancellationToken cancellationToken) =>
        roles.CreateAsync(request.Name.Trim(), cancellationToken);
}
