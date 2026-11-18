namespace Sky.Application.UseCases.City.Queries;

using MediatR;
using Sky.Domain.Ports;
using CityEntity = Sky.Domain.Entities.City;


public sealed record GetCityByIdQuery(int CityId) : IRequest<CityEntity?>;

internal sealed class GetCityByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetCityByIdQuery, CityEntity?>
{
    public Task<CityEntity?> Handle(GetCityByIdQuery request, CancellationToken ct)
    {
        return uow.Repository<CityEntity>().GetByIdAsync(request.CityId, ct);
    }
}