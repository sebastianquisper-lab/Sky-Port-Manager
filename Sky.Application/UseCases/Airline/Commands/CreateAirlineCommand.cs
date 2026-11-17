namespace Sky.Application.UseCases.Airline.Commands;

using MediatR;
using Sky.Domain.Ports;
using AirlineEntity = Sky.Domain.Entities.Airline;


public sealed record CreateAirlineCommand(
    int CountryId,
    string AirlineName,
    string IataCode,
    string IcaoCode,
    string? ContactEmail,
    string? ContactPhone) : IRequest<AirlineEntity>;

internal sealed class CreateAirlineHandler(IUnitOfWork uow)
    : IRequestHandler<CreateAirlineCommand, AirlineEntity>
{
    public async Task<AirlineEntity> Handle(CreateAirlineCommand request, CancellationToken ct)
    {
        var airline = new AirlineEntity
        {
            CountryId = request.CountryId,
            AirlineName = request.AirlineName,
            IataCode = request.IataCode,
            IcaoCode = request.IcaoCode,
            ContactEmail = request.ContactEmail,
            ContactPhone = request.ContactPhone,
            IsActive = true
        };

        await uow.Repository<AirlineEntity>().AddAsync(airline, ct);
        await uow.SaveChangesAsync(ct);

        return airline;
    }
}