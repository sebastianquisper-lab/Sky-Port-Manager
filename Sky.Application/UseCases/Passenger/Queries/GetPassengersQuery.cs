using MediatR;
using Sky.Domain.Ports;
using PassengerEntity = Sky.Domain.Entities.Passenger;

namespace Sky.Application.UseCases.Passenger.Queries;

public sealed record GetPassengersQuery : IRequest<IReadOnlyList<PassengerEntity>>;

internal sealed class GetPassengersHandler(IUnitOfWork uow)
    : IRequestHandler<GetPassengersQuery, IReadOnlyList<PassengerEntity>>
{
    public Task<IReadOnlyList<PassengerEntity>> Handle(GetPassengersQuery request, CancellationToken ct)
        => uow.Repository<PassengerEntity>().GetAllAsync(ct);
}
