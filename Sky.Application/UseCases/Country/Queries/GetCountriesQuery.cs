namespace Sky.Application.UseCases.Country.Queries;

using MediatR;
using Sky.Domain.Ports;
using CountryEntity = Sky.Domain.Entities.Country;


public sealed record GetCountriesQuery : IRequest<IReadOnlyList<CountryEntity>>;

internal sealed class GetCountriesHandler(IUnitOfWork uow)
    : IRequestHandler<GetCountriesQuery, IReadOnlyList<CountryEntity>>
{
    public Task<IReadOnlyList<CountryEntity>> Handle(GetCountriesQuery request, CancellationToken ct)
    {
        return uow.Repository<CountryEntity>().GetAllAsync(ct);
    }
}