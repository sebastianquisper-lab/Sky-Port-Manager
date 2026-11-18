using MediatR;
using Sky.Domain.Ports;
using FareEntity = Sky.Domain.Entities.Fare;

namespace Sky.Application.UseCases.Fare.Queries;

public sealed record GetFaresQuery : IRequest<IReadOnlyList<FareEntity>>;

internal sealed class GetFaresHandler(IUnitOfWork uow)
    : IRequestHandler<GetFaresQuery, IReadOnlyList<FareEntity>>
{
    public Task<IReadOnlyList<FareEntity>> Handle(GetFaresQuery request, CancellationToken ct)
        => uow.Repository<FareEntity>().GetAllAsync(ct);
}
