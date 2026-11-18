using MediatR;
using Sky.Domain.Ports;
using FareEntity = Sky.Domain.Entities.Fare;

namespace Sky.Application.UseCases.Fare.Commands;

public sealed record UpdateFareCommand(
    int FareId,
    int AirlineId,
    int RouteId,
    int CabinClassId,
    string CurrencyCode,
    decimal BasePrice,
    DateOnly ValidFrom,
    DateOnly? ValidTo,
    bool IsActive) : IRequest<FareEntity?>;

internal sealed class UpdateFareHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateFareCommand, FareEntity?>
{
    public async Task<FareEntity?> Handle(UpdateFareCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<FareEntity>().GetByIdAsync(request.FareId, ct);
        if (entity is null) return null;

        entity.AirlineId = request.AirlineId;
        entity.RouteId = request.RouteId;
        entity.CabinClassId = request.CabinClassId;
        entity.CurrencyCode = request.CurrencyCode;
        entity.BasePrice = request.BasePrice;
        entity.ValidFrom = request.ValidFrom;
        entity.ValidTo = request.ValidTo;
        entity.IsActive = request.IsActive;

        uow.Repository<FareEntity>().Update(entity);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
