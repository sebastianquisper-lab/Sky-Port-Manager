namespace Sky.Application.UseCases.CheckInCounter.Queries;

using MediatR;
using Sky.Domain.Ports;
using CheckInCounterEntity = Sky.Domain.Entities.CheckInCounter;


public sealed record GetCheckInCountersQuery : IRequest<List<CheckInCounterEntity>>;

internal sealed class GetCheckInCountersHandler(IUnitOfWork uow)
    : IRequestHandler<GetCheckInCountersQuery, List<CheckInCounterEntity>>
{
    public Task<List<CheckInCounterEntity>> Handle(GetCheckInCountersQuery request, CancellationToken ct)
    {
        return uow.Repository<CheckInCounterEntity>().GetAllAsync(ct);
    }
}