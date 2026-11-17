using MediatR;
using Sky.Domain.Ports;
using AirlineAirportEntity = Sky.Domain.Entities.AirlineAirport;

namespace Sky.Application.UseCases.AirlineAirport.Commands;

public sealed record UpdateAirlineAirportCommand(
    int AirlineId,
    int AirportId,
    DateOnly SinceDate,
    bool IsHub) : IRequest<AirlineAirportEntity?>;

internal sealed class UpdateAirlineAirportHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateAirlineAirportCommand, AirlineAirportEntity?>
{
    public async Task<AirlineAirportEntity?> Handle(UpdateAirlineAirportCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<AirlineAirportEntity>().FirstOrDefaultAsync(x => x.AirlineId == request.AirlineId && x.AirportId == request.AirportId, ct);
        if (entity is null) return null;

        entity.SinceDate = request.SinceDate;
        entity.IsHub = request.IsHub;

        uow.Repository<AirlineAirportEntity>().Update(entity);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
