using MediatR;
using Sky.Domain.Ports;
using FlightRunwayUsageEntity = Sky.Domain.Entities.FlightRunwayUsage;

namespace Sky.Application.UseCases.FlightRunwayUsage.Commands;

public sealed record UpdateFlightRunwayUsageCommand(
    int UsageId,
    int FlightId,
    int RunwayId,
    string Operation,
    DateTime UsedAtUtc) : IRequest<FlightRunwayUsageEntity?>;

internal sealed class UpdateFlightRunwayUsageHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateFlightRunwayUsageCommand, FlightRunwayUsageEntity?>
{
    public async Task<FlightRunwayUsageEntity?> Handle(UpdateFlightRunwayUsageCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<FlightRunwayUsageEntity>().GetByIdAsync(request.UsageId, ct);
        if (entity is null) return null;

        entity.FlightId = request.FlightId;
        entity.RunwayId = request.RunwayId;
        entity.Operation = request.Operation;
        entity.UsedAtUtc = request.UsedAtUtc;

        uow.Repository<FlightRunwayUsageEntity>().Update(entity);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
