using MediatR;
using Sky.Domain.Ports;
using CurrencyEntity = Sky.Domain.Entities.Currency;

namespace Sky.Application.UseCases.Currency.Queries;

public sealed record GetCurrencyByIdQuery(
    string CurrencyCode) : IRequest<CurrencyEntity?>;

internal sealed class GetCurrencyByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetCurrencyByIdQuery, CurrencyEntity?>
{
    public Task<CurrencyEntity?> Handle(GetCurrencyByIdQuery request, CancellationToken ct)
        => uow.Repository<CurrencyEntity>().GetByIdAsync(request.CurrencyCode, ct);
}
