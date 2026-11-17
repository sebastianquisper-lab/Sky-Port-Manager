namespace Sky.Application.UseCases.AppRole.Commands;

using MediatR;
using Sky.Domain.Ports;
using AppRoleEntity = Sky.Domain.Entities.AppRole;


public sealed record CreateAppRoleCommand(
    string RoleName,
    string? Description) : IRequest<AppRoleEntity>;

internal sealed class CreateAppRoleHandler(IUnitOfWork uow)
    : IRequestHandler<CreateAppRoleCommand, AppRoleEntity>
{
    public async Task<AppRoleEntity> Handle(CreateAppRoleCommand request, CancellationToken ct)
    {
        var appRole = new AppRoleEntity
        {
            RoleName = request.RoleName,
            Description = request.Description
        };

        await uow.Repository<AppRoleEntity>().AddAsync(appRole, ct);
        await uow.SaveChangesAsync(ct);

        return appRole;
    }
}