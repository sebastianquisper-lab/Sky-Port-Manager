using MediatR;
using Sky.Domain.Ports;
using FlightStatusEntity = Sky.Domain.Entities.FlightStatus;

namespace Sky.Application.UseCases.FlightStatus.Commands;

public sealed record UpdateFlightStatusCommand(
    int FlightStatusId,
    string StatusName,
    string? Description) : IRequest<FlightStatusEntity?>;

internal sealed class UpdateFlightStatusHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateFlightStatusCommand, FlightStatusEntity?>
{
    public async Task<FlightStatusEntity?> Handle(UpdateFlightStatusCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<FlightStatusEntity>().GetByIdAsync(request.FlightStatusId, ct);
        if (entity is null) return null;

        entity.StatusName = request.StatusName;
        entity.Description = request.Description;

        uow.Repository<FlightStatusEntity>().Update(entity);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
