namespace Sky.Application.UseCases.BaggageStatus.Queries;

using MediatR;
using Sky.Domain.Ports;
using BaggageStatusEntity = Sky.Domain.Entities.BaggageStatus;


public sealed record GetBaggageStatusByIdQuery(int BaggageStatusId) : IRequest<BaggageStatusEntity?>;

internal sealed class GetBaggageStatusByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetBaggageStatusByIdQuery, BaggageStatusEntity?>
{
    public Task<BaggageStatusEntity?> Handle(GetBaggageStatusByIdQuery request, CancellationToken ct)
    {
        return uow.Repository<BaggageStatusEntity>().GetByIdAsync(request.BaggageStatusId, ct);
    }
}