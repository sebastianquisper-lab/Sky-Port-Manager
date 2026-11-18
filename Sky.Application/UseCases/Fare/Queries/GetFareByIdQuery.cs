using MediatR;
using Sky.Domain.Ports;
using FareEntity = Sky.Domain.Entities.Fare;

namespace Sky.Application.UseCases.Fare.Queries;

public sealed record GetFareByIdQuery(
    int FareId) : IRequest<FareEntity?>;

internal sealed class GetFareByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetFareByIdQuery, FareEntity?>
{
    public Task<FareEntity?> Handle(GetFareByIdQuery request, CancellationToken ct)
        => uow.Repository<FareEntity>().GetByIdAsync(request.FareId, ct);
}
