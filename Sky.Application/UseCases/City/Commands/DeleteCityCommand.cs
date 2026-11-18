namespace Sky.Application.UseCases.City.Commands;

using MediatR;
using Sky.Domain.Ports;
using CityEntity = Sky.Domain.Entities.City;


public sealed record DeleteCityCommand(int CityId) : IRequest<bool>;

internal sealed class DeleteCityHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteCityCommand, bool>
{
    public async Task<bool> Handle(DeleteCityCommand request, CancellationToken ct)
    {
        var city = await uow.Repository<CityEntity>().GetByIdAsync(request.CityId, ct);
        if (city is null) return false;

        uow.Repository<CityEntity>().Delete(city);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}