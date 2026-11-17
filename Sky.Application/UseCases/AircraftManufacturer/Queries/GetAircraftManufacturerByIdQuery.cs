using MediatR;
using Sky.Domain.Ports;
using AircraftManufacturerEntity = Sky.Domain.Entities.AircraftManufacturer;

namespace Sky.Application.UseCases.AircraftManufacturer.Queries;

public sealed record GetAircraftManufacturerByIdQuery(
    int ManufacturerId) : IRequest<AircraftManufacturerEntity?>;

internal sealed class GetAircraftManufacturerByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetAircraftManufacturerByIdQuery, AircraftManufacturerEntity?>
{
    public Task<AircraftManufacturerEntity?> Handle(GetAircraftManufacturerByIdQuery request, CancellationToken ct)
        => uow.Repository<AircraftManufacturerEntity>().GetByIdAsync(request.ManufacturerId, ct);
}
