namespace Sky.Application.UseCases.Aircraft.Queries;

using MediatR;
using Sky.Domain.Ports;
using AircraftEntity = Sky.Domain.Entities.Aircraft;


public sealed record GetAircraftByIdQuery(int AircraftId) : IRequest<AircraftEntity?>;

internal sealed class GetAircraftByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetAircraftByIdQuery, AircraftEntity?>
{
    public Task<AircraftEntity?> Handle(GetAircraftByIdQuery request, CancellationToken ct)
    {
        return uow.Repository<AircraftEntity>().GetByIdAsync(request.AircraftId, ct);
    }
}