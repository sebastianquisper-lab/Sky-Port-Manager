using MediatR;
using Sky.Domain.Ports;
using FlightStatusEntity = Sky.Domain.Entities.FlightStatus;

namespace Sky.Application.UseCases.FlightStatus.Commands;

public sealed record CreateFlightStatusCommand(
    string StatusName,
    string? Description) : IRequest<FlightStatusEntity>;

internal sealed class CreateFlightStatusHandler(IUnitOfWork uow)
    : IRequestHandler<CreateFlightStatusCommand, FlightStatusEntity>
{
    public async Task<FlightStatusEntity> Handle(CreateFlightStatusCommand request, CancellationToken ct)
    {
        var entity = new FlightStatusEntity
        {
            StatusName = request.StatusName,
            Description = request.Description
        };

        await uow.Repository<FlightStatusEntity>().AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
