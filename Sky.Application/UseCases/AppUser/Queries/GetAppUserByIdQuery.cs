using MediatR;
using Sky.Domain.Ports;
using AppUserEntity = Sky.Domain.Entities.AppUser;

namespace Sky.Application.UseCases.AppUser.Queries;

public sealed record GetAppUserByIdQuery(
    int UserId) : IRequest<AppUserEntity?>;

internal sealed class GetAppUserByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetAppUserByIdQuery, AppUserEntity?>
{
    public Task<AppUserEntity?> Handle(GetAppUserByIdQuery request, CancellationToken ct)
        => uow.Repository<AppUserEntity>().GetByIdAsync(request.UserId, ct);
}
