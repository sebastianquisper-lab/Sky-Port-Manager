namespace Sky.Application.UseCases.City.Commands;

using MediatR;
using Sky.Domain.Ports;
using CityEntity = Sky.Domain.Entities.City;


public sealed record CreateCityCommand(
    int CountryId,
    string CityName) : IRequest<CityEntity>;

internal sealed class CreateCityHandler(IUnitOfWork uow)
    : IRequestHandler<CreateCityCommand, CityEntity>
{
    public async Task<CityEntity> Handle(CreateCityCommand request, CancellationToken ct)
    {
        var city = new CityEntity
        {
            CountryId = request.CountryId,
            CityName = request.CityName
        };

        await uow.Repository<CityEntity>().AddAsync(city, ct);
        await uow.SaveChangesAsync(ct);

        return city;
    }
}