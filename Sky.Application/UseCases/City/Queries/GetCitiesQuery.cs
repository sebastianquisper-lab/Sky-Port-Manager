namespace Sky.Application.UseCases.City.Queries;

using MediatR;
using Sky.Domain.Ports;
using CityEntity = Sky.Domain.Entities.City;


public sealed record GetCitiesQuery : IRequest<List<CityEntity>>;

internal sealed class GetCitiesHandler(IUnitOfWork uow)
    : IRequestHandler<GetCitiesQuery, List<CityEntity>>
{
    public Task<List<CityEntity>> Handle(GetCitiesQuery request, CancellationToken ct)
    {
        return uow.Repository<CityEntity>().GetAllAsync(ct);
    }
}