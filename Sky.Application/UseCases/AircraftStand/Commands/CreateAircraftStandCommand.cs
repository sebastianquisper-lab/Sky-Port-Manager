namespace Sky.Application.UseCases.AircraftStand.Commands;

using MediatR;
using Sky.Domain.Ports;
using AircraftStandEntity = Sky.Domain.Entities.AircraftStand;


public sealed record CreateAircraftStandCommand(
    int AirportId,
    string StandCode,
    bool IsRemote) : IRequest<AircraftStandEntity>;

internal sealed class CreateAircraftStandHandler(IUnitOfWork uow)
    : IRequestHandler<CreateAircraftStandCommand, AircraftStandEntity>
{
    public async Task<AircraftStandEntity> Handle(CreateAircraftStandCommand request, CancellationToken ct)
    {
        var aircraftStand = new AircraftStandEntity
        {
            AirportId = request.AirportId,
            StandCode = request.StandCode,
            IsRemote = request.IsRemote,
            IsActive = true
        };

        await uow.Repository<AircraftStandEntity>().AddAsync(aircraftStand, ct);
        await uow.SaveChangesAsync(ct);

        return aircraftStand;
    }
}