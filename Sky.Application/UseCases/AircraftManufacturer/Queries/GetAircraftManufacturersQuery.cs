using MediatR;
using Sky.Domain.Ports;
using AircraftManufacturerEntity = Sky.Domain.Entities.AircraftManufacturer;

namespace Sky.Application.UseCases.AircraftManufacturer.Queries;

public sealed record GetAircraftManufacturersQuery : IRequest<IReadOnlyList<AircraftManufacturerEntity>>;

internal sealed class GetAircraftManufacturersHandler(IUnitOfWork uow)
    : IRequestHandler<GetAircraftManufacturersQuery, IReadOnlyList<AircraftManufacturerEntity>>
{
    public Task<IReadOnlyList<AircraftManufacturerEntity>> Handle(GetAircraftManufacturersQuery request, CancellationToken ct)
        => uow.Repository<AircraftManufacturerEntity>().GetAllAsync(ct);
}
