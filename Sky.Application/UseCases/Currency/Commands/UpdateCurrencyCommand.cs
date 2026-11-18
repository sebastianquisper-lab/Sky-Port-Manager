using MediatR;
using Sky.Domain.Ports;
using CurrencyEntity = Sky.Domain.Entities.Currency;

namespace Sky.Application.UseCases.Currency.Commands;

public sealed record UpdateCurrencyCommand(
    string CurrencyCode,
    string CurrencyName,
    string? Symbol) : IRequest<CurrencyEntity?>;

internal sealed class UpdateCurrencyHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateCurrencyCommand, CurrencyEntity?>
{
    public async Task<CurrencyEntity?> Handle(UpdateCurrencyCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<CurrencyEntity>().GetByIdAsync(request.CurrencyCode, ct);
        if (entity is null) return null;

        entity.CurrencyName = request.CurrencyName;
        entity.Symbol = request.Symbol;

        uow.Repository<CurrencyEntity>().Update(entity);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
