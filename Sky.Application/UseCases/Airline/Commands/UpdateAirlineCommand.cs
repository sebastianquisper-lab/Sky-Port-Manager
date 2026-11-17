namespace Sky.Application.UseCases.Airline.Commands;

using MediatR;
using Sky.Domain.Ports;
using AirlineEntity = Sky.Domain.Entities.Airline;


public sealed record UpdateAirlineCommand(
    int AirlineId,
    int CountryId,
    string AirlineName,
    string IataCode,
    string IcaoCode,
    string? ContactEmail,
    string? ContactPhone,
    bool IsActive) : IRequest<AirlineEntity?>;

internal sealed class UpdateAirlineHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateAirlineCommand, AirlineEntity?>
{
    public async Task<AirlineEntity?> Handle(UpdateAirlineCommand request, CancellationToken ct)
    {
        var airline = await uow.Repository<AirlineEntity>().GetByIdAsync(request.AirlineId, ct);
        if (airline is null) return null;

        airline.CountryId = request.CountryId;
        airline.AirlineName = request.AirlineName;
        airline.IataCode = request.IataCode;
        airline.IcaoCode = request.IcaoCode;
        airline.ContactEmail = request.ContactEmail;
        airline.ContactPhone = request.ContactPhone;
        airline.IsActive = request.IsActive;

        uow.Repository<AirlineEntity>().Update(airline);
        await uow.SaveChangesAsync(ct);

        return airline;
    }
}