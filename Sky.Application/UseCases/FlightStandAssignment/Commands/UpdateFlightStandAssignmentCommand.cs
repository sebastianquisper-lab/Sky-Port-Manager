using MediatR;
using Sky.Domain.Ports;
using FlightStandAssignmentEntity = Sky.Domain.Entities.FlightStandAssignment;

namespace Sky.Application.UseCases.FlightStandAssignment.Commands;

public sealed record UpdateFlightStandAssignmentCommand(
    int AssignmentId,
    int FlightId,
    int StandId,
    DateTime AssignedFromUtc,
    DateTime AssignedToUtc) : IRequest<FlightStandAssignmentEntity?>;

internal sealed class UpdateFlightStandAssignmentHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateFlightStandAssignmentCommand, FlightStandAssignmentEntity?>
{
    public async Task<FlightStandAssignmentEntity?> Handle(UpdateFlightStandAssignmentCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<FlightStandAssignmentEntity>().GetByIdAsync(request.AssignmentId, ct);
        if (entity is null) return null;

        entity.FlightId = request.FlightId;
        entity.StandId = request.StandId;
        entity.AssignedFromUtc = request.AssignedFromUtc;
        entity.AssignedToUtc = request.AssignedToUtc;

        uow.Repository<FlightStandAssignmentEntity>().Update(entity);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
