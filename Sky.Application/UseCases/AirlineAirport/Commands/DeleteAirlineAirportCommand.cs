namespace Sky.Application.UseCases.AirlineAirport.Commands;

using MediatR;
using Sky.Domain.Ports;
using AirlineAirportEntity = Sky.Domain.Entities.AirlineAirport;


public sealed record DeleteAirlineAirportCommand(
    int AirlineId,
    int AirportId) : IRequest<bool>;

internal sealed class DeleteAirlineAirportHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteAirlineAirportCommand, bool>
{
    public async Task<bool> Handle(DeleteAirlineAirportCommand request, CancellationToken ct)
    {
        var airlineAirport = await uow.Repository<AirlineAirportEntity>().FirstOrDefaultAsync(x => x.AirlineId == request.AirlineId && x.AirportId == request.AirportId, ct);
        if (airlineAirport is null) return false;

        uow.Repository<AirlineAirportEntity>().Delete(airlineAirport);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}