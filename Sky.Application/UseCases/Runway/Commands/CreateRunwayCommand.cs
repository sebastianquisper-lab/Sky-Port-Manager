using MediatR;
using Sky.Domain.Ports;
using RunwayEntity = Sky.Domain.Entities.Runway;

namespace Sky.Application.UseCases.Runway.Commands;

public sealed record CreateRunwayCommand(
    int AirportId,
    string RunwayCode,
    int LengthM,
    string? SurfaceType) : IRequest<RunwayEntity>;

internal sealed class CreateRunwayHandler(IUnitOfWork uow)
    : IRequestHandler<CreateRunwayCommand, RunwayEntity>
{
    public async Task<RunwayEntity> Handle(CreateRunwayCommand request, CancellationToken ct)
    {
        var entity = new RunwayEntity
        {
            AirportId = request.AirportId,
            RunwayCode = request.RunwayCode,
            LengthM = request.LengthM,
            SurfaceType = request.SurfaceType,
            IsActive = true
        };

        await uow.Repository<RunwayEntity>().AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
