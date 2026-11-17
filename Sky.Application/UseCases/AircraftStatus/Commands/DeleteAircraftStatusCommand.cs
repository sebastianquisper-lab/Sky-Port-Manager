using MediatR;
using Sky.Domain.Ports;
using AircraftStatusEntity = Sky.Domain.Entities.AircraftStatus;

namespace Sky.Application.UseCases.AircraftStatus.Commands;

public sealed record DeleteAircraftStatusCommand(
    int AircraftStatusId) : IRequest<bool>;

internal sealed class DeleteAircraftStatusHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteAircraftStatusCommand, bool>
{
    public async Task<bool> Handle(DeleteAircraftStatusCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<AircraftStatusEntity>().GetByIdAsync(request.AircraftStatusId, ct);
        if (entity is null) return false;

        uow.Repository<AircraftStatusEntity>().Delete(entity);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}
