using MediatR;
using Sky.Domain.Ports;
using AircraftSeatEntity = Sky.Domain.Entities.AircraftSeat;

namespace Sky.Application.UseCases.AircraftSeat.Commands;

public sealed record DeleteAircraftSeatCommand(
    int AircraftSeatId) : IRequest<bool>;

internal sealed class DeleteAircraftSeatHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteAircraftSeatCommand, bool>
{
    public async Task<bool> Handle(DeleteAircraftSeatCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<AircraftSeatEntity>().GetByIdAsync(request.AircraftSeatId, ct);
        if (entity is null) return false;

        uow.Repository<AircraftSeatEntity>().Delete(entity);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}
