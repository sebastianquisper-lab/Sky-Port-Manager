using MediatR;
using Sky.Domain.Ports;
using AircraftStandEntity = Sky.Domain.Entities.AircraftStand;

namespace Sky.Application.UseCases.AircraftStand.Commands;

public sealed record CreateAircraftStandCommand(
    int AirportId,
    string StandCode,
    bool IsRemote) : IRequest<AircraftStandEntity>;

internal sealed class CreateAircraftStandHandler(IUnitOfWork uow)
    : IRequestHandler<CreateAircraftStandCommand, AircraftStandEntity>
{
    public async Task<AircraftStandEntity> Handle(CreateAircraftStandCommand request, CancellationToken ct)
    {
        var entity = new AircraftStandEntity
        {
            AirportId = request.AirportId,
            StandCode = request.StandCode,
            IsRemote = request.IsRemote,
            IsActive = true
        };

        await uow.Repository<AircraftStandEntity>().AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
