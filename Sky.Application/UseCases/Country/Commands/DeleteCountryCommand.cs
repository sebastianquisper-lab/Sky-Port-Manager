namespace Sky.Application.UseCases.Country.Commands;

using MediatR;
using Sky.Domain.Ports;
using CountryEntity = Sky.Domain.Entities.Country;


public sealed record DeleteCountryCommand(int CountryId) : IRequest<bool>;

internal sealed class DeleteCountryHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteCountryCommand, bool>
{
    public async Task<bool> Handle(DeleteCountryCommand request, CancellationToken ct)
    {
        var country = await uow.Repository<CountryEntity>().GetByIdAsync(request.CountryId, ct);
        if (country is null) return false;

        uow.Repository<CountryEntity>().Delete(country);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}