namespace Sky.Application.UseCases.Country.Commands;

using MediatR;
using Sky.Domain.Ports;
using CountryEntity = Sky.Domain.Entities.Country;


public sealed record UpdateCountryCommand(
    int CountryId,
    string CountryName,
    string Iso2Code,
    string Iso3Code) : IRequest<CountryEntity?>;

internal sealed class UpdateCountryHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateCountryCommand, CountryEntity?>
{
    public async Task<CountryEntity?> Handle(UpdateCountryCommand request, CancellationToken ct)
    {
        var country = await uow.Repository<CountryEntity>().GetByIdAsync(request.CountryId, ct);
        if (country is null) return null;

        country.CountryName = request.CountryName;
        country.Iso2Code = request.Iso2Code;
        country.Iso3Code = request.Iso3Code;

        uow.Repository<CountryEntity>().Update(country);
        await uow.SaveChangesAsync(ct);

        return country;
    }
}