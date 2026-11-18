using MediatR;
using Sky.Domain.Ports;
using FlightStandAssignmentEntity = Sky.Domain.Entities.FlightStandAssignment;

namespace Sky.Application.UseCases.FlightStandAssignment.Commands;

public sealed record CreateFlightStandAssignmentCommand(
    int FlightId,
    int StandId,
    DateTime AssignedFromUtc,
    DateTime AssignedToUtc) : IRequest<FlightStandAssignmentEntity>;

internal sealed class CreateFlightStandAssignmentHandler(IUnitOfWork uow)
    : IRequestHandler<CreateFlightStandAssignmentCommand, FlightStandAssignmentEntity>
{
    public async Task<FlightStandAssignmentEntity> Handle(CreateFlightStandAssignmentCommand request, CancellationToken ct)
    {
        var entity = new FlightStandAssignmentEntity
        {
            FlightId = request.FlightId,
            StandId = request.StandId,
            AssignedFromUtc = request.AssignedFromUtc,
            AssignedToUtc = request.AssignedToUtc
        };

        await uow.Repository<FlightStandAssignmentEntity>().AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
