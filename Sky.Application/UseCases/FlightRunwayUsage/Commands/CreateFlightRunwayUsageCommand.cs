using MediatR;
using Sky.Domain.Ports;
using FlightRunwayUsageEntity = Sky.Domain.Entities.FlightRunwayUsage;

namespace Sky.Application.UseCases.FlightRunwayUsage.Commands;

public sealed record CreateFlightRunwayUsageCommand(
    int FlightId,
    int RunwayId,
    string Operation,
    DateTime UsedAtUtc) : IRequest<FlightRunwayUsageEntity>;

internal sealed class CreateFlightRunwayUsageHandler(IUnitOfWork uow)
    : IRequestHandler<CreateFlightRunwayUsageCommand, FlightRunwayUsageEntity>
{
    public async Task<FlightRunwayUsageEntity> Handle(CreateFlightRunwayUsageCommand request, CancellationToken ct)
    {
        var entity = new FlightRunwayUsageEntity
        {
            FlightId = request.FlightId,
            RunwayId = request.RunwayId,
            Operation = request.Operation,
            UsedAtUtc = request.UsedAtUtc
        };

        await uow.Repository<FlightRunwayUsageEntity>().AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
