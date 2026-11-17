using MediatR;
using Sky.Domain.Ports;
using AirportEntity = Sky.Domain.Entities.Airport;

namespace Sky.Application.UseCases.Airport.Commands;

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
        var entity = await uow.Repository<AirportEntity>().GetByIdAsync(request.AirportId, ct);
        if (entity is null) return null;

        entity.CityId = request.CityId;
        entity.AirportName = request.AirportName;
        entity.IataCode = request.IataCode;
        entity.IcaoCode = request.IcaoCode;
        entity.Address = request.Address;
        entity.TimeZone = request.TimeZone;
        entity.Latitude = request.Latitude;
        entity.Longitude = request.Longitude;
        entity.IsActive = request.IsActive;

        uow.Repository<AirportEntity>().Update(entity);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
