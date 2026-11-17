using MediatR;
using Sky.Domain.Ports;
using AirlineEntity = Sky.Domain.Entities.Airline;

namespace Sky.Application.UseCases.Airline.Queries;

public sealed record GetAirlinesQuery : IRequest<IReadOnlyList<AirlineEntity>>;

internal sealed class GetAirlinesHandler(IUnitOfWork uow)
    : IRequestHandler<GetAirlinesQuery, IReadOnlyList<AirlineEntity>>
{
    public Task<IReadOnlyList<AirlineEntity>> Handle(GetAirlinesQuery request, CancellationToken ct)
        => uow.Repository<AirlineEntity>().GetAllAsync(ct);
}
