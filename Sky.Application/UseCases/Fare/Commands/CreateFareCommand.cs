using MediatR;
using Sky.Domain.Ports;
using FareEntity = Sky.Domain.Entities.Fare;

namespace Sky.Application.UseCases.Fare.Commands;

public sealed record CreateFareCommand(
    int AirlineId,
    int RouteId,
    int CabinClassId,
    string CurrencyCode,
    decimal BasePrice,
    DateOnly ValidFrom,
    DateOnly? ValidTo) : IRequest<FareEntity>;

internal sealed class CreateFareHandler(IUnitOfWork uow)
    : IRequestHandler<CreateFareCommand, FareEntity>
{
    public async Task<FareEntity> Handle(CreateFareCommand request, CancellationToken ct)
    {
        var entity = new FareEntity
        {
            AirlineId = request.AirlineId,
            RouteId = request.RouteId,
            CabinClassId = request.CabinClassId,
            CurrencyCode = request.CurrencyCode,
            BasePrice = request.BasePrice,
            ValidFrom = request.ValidFrom,
            ValidTo = request.ValidTo,
            IsActive = true
        };

        await uow.Repository<FareEntity>().AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
