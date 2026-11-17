using MediatR;
using Sky.Domain.Ports;
using AircraftEntity = Sky.Domain.Entities.Aircraft;

namespace Sky.Application.UseCases.Aircraft.Commands;

public sealed record DeleteAircraftCommand(
    int AircraftId) : IRequest<bool>;

internal sealed class DeleteAircraftHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteAircraftCommand, bool>
{
    public async Task<bool> Handle(DeleteAircraftCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<AircraftEntity>().GetByIdAsync(request.AircraftId, ct);
        if (entity is null) return false;

        uow.Repository<AircraftEntity>().Delete(entity);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}
