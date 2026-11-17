namespace Sky.Application.UseCases.AircraftManufacturer.Queries;

using MediatR;
using Sky.Domain.Ports;
using AircraftManufacturerEntity = Sky.Domain.Entities.AircraftManufacturer;


public sealed record GetAircraftManufacturersQuery : IRequest<List<AircraftManufacturerEntity>>;

internal sealed class GetAircraftManufacturersHandler(IUnitOfWork uow)
    : IRequestHandler<GetAircraftManufacturersQuery, List<AircraftManufacturerEntity>>
{
    public Task<List<AircraftManufacturerEntity>> Handle(GetAircraftManufacturersQuery request, CancellationToken ct)
    {
        return uow.Repository<AircraftManufacturerEntity>().GetAllAsync(ct);
    }
}