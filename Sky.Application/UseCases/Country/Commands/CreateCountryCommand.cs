namespace Sky.Application.UseCases.Country.Commands;

using MediatR;
using Sky.Domain.Ports;
using CountryEntity = Sky.Domain.Entities.Country;


public sealed record CreateCountryCommand(
    string CountryName,
    string Iso2Code,
    string Iso3Code) : IRequest<CountryEntity>;

internal sealed class CreateCountryHandler(IUnitOfWork uow)
    : IRequestHandler<CreateCountryCommand, CountryEntity>
{
    public async Task<CountryEntity> Handle(CreateCountryCommand request, CancellationToken ct)
    {
        var country = new CountryEntity
        {
            CountryName = request.CountryName,
            Iso2Code = request.Iso2Code,
            Iso3Code = request.Iso3Code
        };

        await uow.Repository<CountryEntity>().AddAsync(country, ct);
        await uow.SaveChangesAsync(ct);

        return country;
    }
}