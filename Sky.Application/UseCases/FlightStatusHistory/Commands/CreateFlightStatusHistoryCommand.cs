using MediatR;
using Sky.Application.Ports;
using Sky.Domain.Ports;
using FlightStatusHistoryEntity = Sky.Domain.Entities.FlightStatusHistory;

namespace Sky.Application.UseCases.FlightStatusHistory.Commands;

public sealed record CreateFlightStatusHistoryCommand(
    int FlightId,
    int FlightStatusId,
    string? Notes) : IRequest<FlightStatusHistoryEntity>;

internal sealed class CreateFlightStatusHistoryHandler(
    IUnitOfWork uow,
    IDateTimeProvider clock,
    ICurrentUserService currentUser)
    : IRequestHandler<CreateFlightStatusHistoryCommand, FlightStatusHistoryEntity>
{
    public async Task<FlightStatusHistoryEntity> Handle(CreateFlightStatusHistoryCommand request, CancellationToken ct)
    {
        var entity = new FlightStatusHistoryEntity
        {
            FlightId = request.FlightId,
            FlightStatusId = request.FlightStatusId,
            ChangedByEmployeeId = currentUser.EmployeeId,
            ChangedAtUtc = clock.UtcNow,
            Notes = request.Notes
        };

        await uow.Repository<FlightStatusHistoryEntity>().AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
