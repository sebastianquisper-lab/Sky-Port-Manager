using MediatR;
using Sky.Domain.Ports;
using CurrencyEntity = Sky.Domain.Entities.Currency;

namespace Sky.Application.UseCases.Currency.Commands;

public sealed record DeleteCurrencyCommand(
    string CurrencyCode) : IRequest<bool>;

internal sealed class DeleteCurrencyHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteCurrencyCommand, bool>
{
    public async Task<bool> Handle(DeleteCurrencyCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<CurrencyEntity>().GetByIdAsync(request.CurrencyCode, ct);
        if (entity is null) return false;

        uow.Repository<CurrencyEntity>().Delete(entity);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}
