using MediatR;
using Sky.Domain.Ports;
using FlightEntity = Sky.Domain.Entities.Flight;

namespace Sky.Application.UseCases.Flight.Commands;

public sealed record DeleteFlightCommand(
    int FlightId) : IRequest<bool>;

internal sealed class DeleteFlightHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteFlightCommand, bool>
{
    public async Task<bool> Handle(DeleteFlightCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<FlightEntity>().GetByIdAsync(request.FlightId, ct);
        if (entity is null) return false;

        uow.Repository<FlightEntity>().Delete(entity);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}
