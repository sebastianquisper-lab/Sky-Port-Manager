namespace Sky.Application.UseCases.AirlineAirport.Commands;

using MediatR;
using Sky.Domain.Ports;
using AirlineAirportEntity = Sky.Domain.Entities.AirlineAirport;


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
        var airlineAirport = await uow.Repository<AirlineAirportEntity>().FirstOrDefaultAsync(x => x.AirlineId == request.AirlineId && x.AirportId == request.AirportId, ct);
        if (airlineAirport is null) return null;

        airlineAirport.SinceDate = request.SinceDate;
        airlineAirport.IsHub = request.IsHub;

        uow.Repository<AirlineAirportEntity>().Update(airlineAirport);
        await uow.SaveChangesAsync(ct);

        return airlineAirport;
    }
}