namespace Sky.Application.UseCases.Aircraft.Commands;

using MediatR;
using Sky.Domain.Ports;
using AircraftEntity = Sky.Domain.Entities.Aircraft;


public sealed record UpdateAircraftCommand(
    int AircraftId,
    int AirlineId,
    int ModelId,
    int AircraftStatusId,
    string RegistrationCode,
    string? SerialNumber,
    short SeatCapacity,
    short? ManufactureYear,
    bool IsActive) : IRequest<AircraftEntity?>;

internal sealed class UpdateAircraftHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateAircraftCommand, AircraftEntity?>
{
    public async Task<AircraftEntity?> Handle(UpdateAircraftCommand request, CancellationToken ct)
    {
        var aircraft = await uow.Repository<AircraftEntity>().GetByIdAsync(request.AircraftId, ct);
        if (aircraft is null) return null;

        aircraft.AirlineId = request.AirlineId;
        aircraft.ModelId = request.ModelId;
        aircraft.AircraftStatusId = request.AircraftStatusId;
        aircraft.RegistrationCode = request.RegistrationCode;
        aircraft.SerialNumber = request.SerialNumber;
        aircraft.SeatCapacity = request.SeatCapacity;
        aircraft.ManufactureYear = request.ManufactureYear;
        aircraft.IsActive = request.IsActive;

        uow.Repository<AircraftEntity>().Update(aircraft);
        await uow.SaveChangesAsync(ct);

        return aircraft;
    }
}