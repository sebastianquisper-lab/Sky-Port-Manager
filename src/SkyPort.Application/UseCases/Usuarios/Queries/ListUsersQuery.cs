using MediatR;
using SkyPort.Application.Ports;
using SkyPort.Application.UseCases.Usuarios.Dtos;

namespace SkyPort.Application.UseCases.Usuarios.Queries;

public sealed record ListUsersQuery : IRequest<IReadOnlyList<UserDto>>;

public sealed class ListUsersQueryHandler(IUserAccountService userAccountService)
    : IRequestHandler<ListUsersQuery, IReadOnlyList<UserDto>>
{
    public Task<IReadOnlyList<UserDto>> Handle(ListUsersQuery request, CancellationToken cancellationToken) =>
        userAccountService.ListAsync(cancellationToken);
}
