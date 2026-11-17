using MediatR;
using Sky.Domain.Ports;
using AirlineEntity = Sky.Domain.Entities.Airline;

namespace Sky.Application.UseCases.Airline.Commands;

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
        var entity = await uow.Repository<AirlineEntity>().GetByIdAsync(request.AirlineId, ct);
        if (entity is null) return null;

        entity.CountryId = request.CountryId;
        entity.AirlineName = request.AirlineName;
        entity.IataCode = request.IataCode;
        entity.IcaoCode = request.IcaoCode;
        entity.ContactEmail = request.ContactEmail;
        entity.ContactPhone = request.ContactPhone;
        entity.IsActive = request.IsActive;

        uow.Repository<AirlineEntity>().Update(entity);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
