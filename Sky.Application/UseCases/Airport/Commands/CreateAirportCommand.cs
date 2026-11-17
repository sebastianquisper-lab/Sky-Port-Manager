using MediatR;
using Sky.Domain.Ports;
using AirportEntity = Sky.Domain.Entities.Airport;

namespace Sky.Application.UseCases.Airport.Commands;

public sealed record CreateAirportCommand(
    int CityId,
    string AirportName,
    string IataCode,
    string IcaoCode,
    string? Address,
    string TimeZone,
    decimal? Latitude,
    decimal? Longitude) : IRequest<AirportEntity>;

internal sealed class CreateAirportHandler(IUnitOfWork uow)
    : IRequestHandler<CreateAirportCommand, AirportEntity>
{
    public async Task<AirportEntity> Handle(CreateAirportCommand request, CancellationToken ct)
    {
        var entity = new AirportEntity
        {
            CityId = request.CityId,
            AirportName = request.AirportName,
            IataCode = request.IataCode,
            IcaoCode = request.IcaoCode,
            Address = request.Address,
            TimeZone = request.TimeZone,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            IsActive = true
        };

        await uow.Repository<AirportEntity>().AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
