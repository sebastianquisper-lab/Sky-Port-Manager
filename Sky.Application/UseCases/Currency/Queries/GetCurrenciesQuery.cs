using MediatR;
using Sky.Domain.Ports;
using CurrencyEntity = Sky.Domain.Entities.Currency;

namespace Sky.Application.UseCases.Currency.Queries;

public sealed record GetCurrenciesQuery : IRequest<IReadOnlyList<CurrencyEntity>>;

internal sealed class GetCurrenciesHandler(IUnitOfWork uow)
    : IRequestHandler<GetCurrenciesQuery, IReadOnlyList<CurrencyEntity>>
{
    public Task<IReadOnlyList<CurrencyEntity>> Handle(GetCurrenciesQuery request, CancellationToken ct)
        => uow.Repository<CurrencyEntity>().GetAllAsync(ct);
}
