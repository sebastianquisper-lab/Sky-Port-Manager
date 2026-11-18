using MediatR;
using Sky.Domain.Ports;
using FlightDelayEntity = Sky.Domain.Entities.FlightDelay;

namespace Sky.Application.UseCases.FlightDelay.Commands;

public sealed record DeleteFlightDelayCommand(
    int FlightDelayId) : IRequest<bool>;

internal sealed class DeleteFlightDelayHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteFlightDelayCommand, bool>
{
    public async Task<bool> Handle(DeleteFlightDelayCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<FlightDelayEntity>().GetByIdAsync(request.FlightDelayId, ct);
        if (entity is null) return false;

        uow.Repository<FlightDelayEntity>().Delete(entity);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}
