namespace Sky.Application.UseCases.BaggageType.Queries;

using MediatR;
using Sky.Domain.Ports;
using BaggageTypeEntity = Sky.Domain.Entities.BaggageType;


public sealed record GetBaggageTypeByIdQuery(int BaggageTypeId) : IRequest<BaggageTypeEntity?>;

internal sealed class GetBaggageTypeByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetBaggageTypeByIdQuery, BaggageTypeEntity?>
{
    public Task<BaggageTypeEntity?> Handle(GetBaggageTypeByIdQuery request, CancellationToken ct)
    {
        return uow.Repository<BaggageTypeEntity>().GetByIdAsync(request.BaggageTypeId, ct);
    }
}