namespace Sky.Application.UseCases.Airline.Queries;

using MediatR;
using Sky.Domain.Ports;
using AirlineEntity = Sky.Domain.Entities.Airline;


public sealed record GetAirlinesQuery : IRequest<List<AirlineEntity>>;

internal sealed class GetAirlinesHandler(IUnitOfWork uow)
    : IRequestHandler<GetAirlinesQuery, List<AirlineEntity>>
{
    public Task<List<AirlineEntity>> Handle(GetAirlinesQuery request, CancellationToken ct)
    {
        return uow.Repository<AirlineEntity>().GetAllAsync(ct);
    }
}