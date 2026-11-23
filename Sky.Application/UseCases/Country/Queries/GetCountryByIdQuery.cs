namespace Sky.Application.UseCases.Country.Queries;

using MediatR;
using Sky.Domain.Ports;
using CountryEntity = Sky.Domain.Entities.Country;


public sealed record GetCountryByIdQuery(int CountryId) : IRequest<CountryEntity?>;

internal sealed class GetCountryByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetCountryByIdQuery, CountryEntity?>
{
    public Task<CountryEntity?> Handle(GetCountryByIdQuery request, CancellationToken ct)
    {
        return uow.Repository<CountryEntity>().GetByIdAsync(request.CountryId, ct);
    }
}
