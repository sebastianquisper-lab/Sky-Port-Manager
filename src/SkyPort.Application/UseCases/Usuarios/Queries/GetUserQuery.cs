using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Ports;
using SkyPort.Application.UseCases.Usuarios.Dtos;

namespace SkyPort.Application.UseCases.Usuarios.Queries;

public sealed record GetUserQuery(string UserId) : IRequest<Result<UserDto>>;

public sealed class GetUserQueryHandler(IUserAccountService userAccountService)
    : IRequestHandler<GetUserQuery, Result<UserDto>>
{
    public Task<Result<UserDto>> Handle(GetUserQuery request, CancellationToken cancellationToken) =>
        userAccountService.GetAsync(request.UserId, cancellationToken);
}
