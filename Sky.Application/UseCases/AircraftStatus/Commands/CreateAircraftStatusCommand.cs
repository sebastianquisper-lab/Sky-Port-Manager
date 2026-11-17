namespace Sky.Application.UseCases.AircraftStatus.Commands;

using MediatR;
using Sky.Domain.Ports;
using AircraftStatusEntity = Sky.Domain.Entities.AircraftStatus;


public sealed record CreateAircraftStatusCommand(
    string StatusName,
    string? Description) : IRequest<AircraftStatusEntity>;

internal sealed class CreateAircraftStatusHandler(IUnitOfWork uow)
    : IRequestHandler<CreateAircraftStatusCommand, AircraftStatusEntity>
{
    public async Task<AircraftStatusEntity> Handle(CreateAircraftStatusCommand request, CancellationToken ct)
    {
        var aircraftStatus = new AircraftStatusEntity
        {
            StatusName = request.StatusName,
            Description = request.Description
        };

        await uow.Repository<AircraftStatusEntity>().AddAsync(aircraftStatus, ct);
        await uow.SaveChangesAsync(ct);

        return aircraftStatus;
    }
}