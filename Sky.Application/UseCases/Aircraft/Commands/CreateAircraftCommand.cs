namespace Sky.Application.UseCases.Aircraft.Commands;

using MediatR;
using Sky.Domain.Ports;
using AircraftEntity = Sky.Domain.Entities.Aircraft;


public sealed record CreateAircraftCommand(
    int AirlineId,
    int ModelId,
    int AircraftStatusId,
    string RegistrationCode,
    string? SerialNumber,
    short SeatCapacity,
    short? ManufactureYear) : IRequest<AircraftEntity>;

internal sealed class CreateAircraftHandler(IUnitOfWork uow)
    : IRequestHandler<CreateAircraftCommand, AircraftEntity>
{
    public async Task<AircraftEntity> Handle(CreateAircraftCommand request, CancellationToken ct)
    {
        var aircraft = new AircraftEntity
        {
            AirlineId = request.AirlineId,
            ModelId = request.ModelId,
            AircraftStatusId = request.AircraftStatusId,
            RegistrationCode = request.RegistrationCode,
            SerialNumber = request.SerialNumber,
            SeatCapacity = request.SeatCapacity,
            ManufactureYear = request.ManufactureYear,
            IsActive = true
        };

        await uow.Repository<AircraftEntity>().AddAsync(aircraft, ct);
        await uow.SaveChangesAsync(ct);

        return aircraft;
    }
}