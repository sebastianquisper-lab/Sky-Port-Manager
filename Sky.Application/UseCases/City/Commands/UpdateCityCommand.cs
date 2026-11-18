namespace Sky.Application.UseCases.City.Commands;

using MediatR;
using Sky.Domain.Ports;
using CityEntity = Sky.Domain.Entities.City;


public sealed record UpdateCityCommand(
    int CityId,
    int CountryId,
    string CityName) : IRequest<CityEntity?>;

internal sealed class UpdateCityHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateCityCommand, CityEntity?>
{
    public async Task<CityEntity?> Handle(UpdateCityCommand request, CancellationToken ct)
    {
        var city = await uow.Repository<CityEntity>().GetByIdAsync(request.CityId, ct);
        if (city is null) return null;

        city.CountryId = request.CountryId;
        city.CityName = request.CityName;

        uow.Repository<CityEntity>().Update(city);
        await uow.SaveChangesAsync(ct);

        return city;
    }
}