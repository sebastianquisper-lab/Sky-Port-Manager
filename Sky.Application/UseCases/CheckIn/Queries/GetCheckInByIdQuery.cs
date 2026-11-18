namespace Sky.Application.UseCases.CheckIn.Queries;

using MediatR;
using Sky.Domain.Ports;
using CheckInEntity = Sky.Domain.Entities.CheckIn;


public sealed record GetCheckInByIdQuery(int CheckInId) : IRequest<CheckInEntity?>;

internal sealed class GetCheckInByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetCheckInByIdQuery, CheckInEntity?>
{
    public Task<CheckInEntity?> Handle(GetCheckInByIdQuery request, CancellationToken ct)
    {
        return uow.Repository<CheckInEntity>().GetByIdAsync(request.CheckInId, ct);
    }
}