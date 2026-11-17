namespace Sky.Application.UseCases.Airport.Queries;

using MediatR;
using Sky.Domain.Ports;
using AirportEntity = Sky.Domain.Entities.Airport;


public sealed record GetAirportByIdQuery(int AirportId) : IRequest<AirportEntity?>;

internal sealed class GetAirportByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetAirportByIdQuery, AirportEntity?>
{
    public Task<AirportEntity?> Handle(GetAirportByIdQuery request, CancellationToken ct)
    {
        return uow.Repository<AirportEntity>().GetByIdAsync(request.AirportId, ct);
    }
}