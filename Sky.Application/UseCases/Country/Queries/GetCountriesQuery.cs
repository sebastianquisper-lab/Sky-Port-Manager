namespace Sky.Application.UseCases.Country.Queries;

using MediatR;
using Sky.Domain.Ports;
using CountryEntity = Sky.Domain.Entities.Country;


public sealed record GetCountriesQuery : IRequest<List<CountryEntity>>;

internal sealed class GetCountriesHandler(IUnitOfWork uow)
    : IRequestHandler<GetCountriesQuery, List<CountryEntity>>
{
    public Task<List<CountryEntity>> Handle(GetCountriesQuery request, CancellationToken ct)
    {
        return uow.Repository<CountryEntity>().GetAllAsync(ct);
    }
}