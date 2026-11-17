using MediatR;
using Sky.Domain.Ports;
using AircraftStatusEntity = Sky.Domain.Entities.AircraftStatus;

namespace Sky.Application.UseCases.AircraftStatus.Commands;

public sealed record CreateAircraftStatusCommand(
    string StatusName,
    string? Description) : IRequest<AircraftStatusEntity>;

internal sealed class CreateAircraftStatusHandler(IUnitOfWork uow)
    : IRequestHandler<CreateAircraftStatusCommand, AircraftStatusEntity>
{
    public async Task<AircraftStatusEntity> Handle(CreateAircraftStatusCommand request, CancellationToken ct)
    {
        var entity = new AircraftStatusEntity
        {
            StatusName = request.StatusName,
            Description = request.Description
        };

        await uow.Repository<AircraftStatusEntity>().AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
