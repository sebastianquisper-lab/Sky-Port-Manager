using MediatR;
using Sky.Domain.Ports;
using FlightRunwayUsageEntity = Sky.Domain.Entities.FlightRunwayUsage;

namespace Sky.Application.UseCases.FlightRunwayUsage.Commands;

public sealed record DeleteFlightRunwayUsageCommand(
    int UsageId) : IRequest<bool>;

internal sealed class DeleteFlightRunwayUsageHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteFlightRunwayUsageCommand, bool>
{
    public async Task<bool> Handle(DeleteFlightRunwayUsageCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<FlightRunwayUsageEntity>().GetByIdAsync(request.UsageId, ct);
        if (entity is null) return false;

        uow.Repository<FlightRunwayUsageEntity>().Delete(entity);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}
