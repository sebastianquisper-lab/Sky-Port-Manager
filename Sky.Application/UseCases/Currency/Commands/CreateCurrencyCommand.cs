using MediatR;
using Sky.Domain.Ports;
using CurrencyEntity = Sky.Domain.Entities.Currency;

namespace Sky.Application.UseCases.Currency.Commands;

public sealed record CreateCurrencyCommand(
    string CurrencyCode,
    string CurrencyName,
    string? Symbol) : IRequest<CurrencyEntity>;

internal sealed class CreateCurrencyHandler(IUnitOfWork uow)
    : IRequestHandler<CreateCurrencyCommand, CurrencyEntity>
{
    public async Task<CurrencyEntity> Handle(CreateCurrencyCommand request, CancellationToken ct)
    {
        var entity = new CurrencyEntity
        {
            CurrencyCode = request.CurrencyCode,
            CurrencyName = request.CurrencyName,
            Symbol = request.Symbol
        };

        await uow.Repository<CurrencyEntity>().AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
