namespace Sky.Application.UseCases.AircraftStatus.Commands;

using MediatR;
using Sky.Domain.Ports;
using AircraftStatusEntity = Sky.Domain.Entities.AircraftStatus;


public sealed record DeleteAircraftStatusCommand(int AircraftStatusId) : IRequest<bool>;

internal sealed class DeleteAircraftStatusHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteAircraftStatusCommand, bool>
{
    public async Task<bool> Handle(DeleteAircraftStatusCommand request, CancellationToken ct)
    {
        var aircraftStatus = await uow.Repository<AircraftStatusEntity>().GetByIdAsync(request.AircraftStatusId, ct);
        if (aircraftStatus is null) return false;

        uow.Repository<AircraftStatusEntity>().Delete(aircraftStatus);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}