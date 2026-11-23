namespace Sky.Application.UseCases.City.Queries;

using MediatR;
using Sky.Domain.Ports;
using CityEntity = Sky.Domain.Entities.City;


public sealed record GetCitiesQuery : IRequest<IReadOnlyList<CityEntity>>;

internal sealed class GetCitiesHandler(IUnitOfWork uow)
    : IRequestHandler<GetCitiesQuery, IReadOnlyList<CityEntity>>
{
    public Task<IReadOnlyList<CityEntity>> Handle(GetCitiesQuery request, CancellationToken ct)
    {
        return uow.Repository<CityEntity>().GetAllAsync(ct);
    }
}