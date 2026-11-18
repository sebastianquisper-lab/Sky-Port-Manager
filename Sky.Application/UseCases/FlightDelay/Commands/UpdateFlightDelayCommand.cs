using MediatR;
using Sky.Domain.Ports;
using FlightDelayEntity = Sky.Domain.Entities.FlightDelay;

namespace Sky.Application.UseCases.FlightDelay.Commands;

public sealed record UpdateFlightDelayCommand(
    int FlightDelayId,
    int FlightId,
    int DelayReasonId,
    int DelayMinutes,
    string? Notes) : IRequest<FlightDelayEntity?>;

internal sealed class UpdateFlightDelayHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateFlightDelayCommand, FlightDelayEntity?>
{
    public async Task<FlightDelayEntity?> Handle(UpdateFlightDelayCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<FlightDelayEntity>().GetByIdAsync(request.FlightDelayId, ct);
        if (entity is null) return null;

        entity.FlightId = request.FlightId;
        entity.DelayReasonId = request.DelayReasonId;
        entity.DelayMinutes = request.DelayMinutes;
        entity.Notes = request.Notes;

        uow.Repository<FlightDelayEntity>().Update(entity);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
