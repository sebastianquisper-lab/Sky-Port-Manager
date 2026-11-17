namespace Sky.Application.UseCases.AircraftModel.Queries;

using MediatR;
using Sky.Domain.Ports;
using AircraftModelEntity = Sky.Domain.Entities.AircraftModel;


public sealed record GetAircraftModelByIdQuery(int ModelId) : IRequest<AircraftModelEntity?>;

internal sealed class GetAircraftModelByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetAircraftModelByIdQuery, AircraftModelEntity?>
{
    public Task<AircraftModelEntity?> Handle(GetAircraftModelByIdQuery request, CancellationToken ct)
    {
        return uow.Repository<AircraftModelEntity>().GetByIdAsync(request.ModelId, ct);
    }
}