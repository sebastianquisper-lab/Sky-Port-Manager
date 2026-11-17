namespace Sky.Application.UseCases.AircraftStand.Commands;

using MediatR;
using Sky.Domain.Ports;
using AircraftStandEntity = Sky.Domain.Entities.AircraftStand;


public sealed record DeleteAircraftStandCommand(int StandId) : IRequest<bool>;

internal sealed class DeleteAircraftStandHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteAircraftStandCommand, bool>
{
    public async Task<bool> Handle(DeleteAircraftStandCommand request, CancellationToken ct)
    {
        var aircraftStand = await uow.Repository<AircraftStandEntity>().GetByIdAsync(request.StandId, ct);
        if (aircraftStand is null) return false;

        uow.Repository<AircraftStandEntity>().Delete(aircraftStand);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}