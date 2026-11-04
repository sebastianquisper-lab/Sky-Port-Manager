using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Ports;

namespace SkyPort.Application.UseCases.Usuarios.Commands;

public sealed record CreateUserCommand(string Email, string NombreCompleto, string Password, string Role) : IRequest<Result<string>>;

public sealed class CreateUserCommandHandler(IUserAccountService userAccountService)
    : IRequestHandler<CreateUserCommand, Result<string>>
{
    public Task<Result<string>> Handle(CreateUserCommand request, CancellationToken cancellationToken) =>
        userAccountService.CreateAsync(request.Email, request.NombreCompleto, request.Password, request.Role, cancellationToken);
}
