namespace Sky.Application.UseCases.Airport.Commands;

using MediatR;
using Sky.Domain.Ports;
using AirportEntity = Sky.Domain.Entities.Airport;


public sealed record UpdateAirportCommand(
    int AirportId,
    int CityId,
    string AirportName,
    string IataCode,
    string IcaoCode,
    string? Address,
    string TimeZone,
    decimal? Latitude,
    decimal? Longitude,
    bool IsActive) : IRequest<AirportEntity?>;

internal sealed class UpdateAirportHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateAirportCommand, AirportEntity?>
{
    public async Task<AirportEntity?> Handle(UpdateAirportCommand request, CancellationToken ct)
    {
        var airport = await uow.Repository<AirportEntity>().GetByIdAsync(request.AirportId, ct);
        if (airport is null) return null;

        airport.CityId = request.CityId;
        airport.AirportName = request.AirportName;
        airport.IataCode = request.IataCode;
        airport.IcaoCode = request.IcaoCode;
        airport.Address = request.Address;
        airport.TimeZone = request.TimeZone;
        airport.Latitude = request.Latitude;
        airport.Longitude = request.Longitude;
        airport.IsActive = request.IsActive;

        uow.Repository<AirportEntity>().Update(airport);
        await uow.SaveChangesAsync(ct);

        return airport;
    }
}