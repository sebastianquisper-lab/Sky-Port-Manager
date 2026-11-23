namespace Sky.Application.UseCases.BookingFlightPassenger.Queries;

using MediatR;
using Sky.Domain.Ports;
using BookingFlightPassengerEntity = Sky.Domain.Entities.BookingFlightPassenger;


public sealed record GetBookingFlightPassengersQuery : IRequest<IReadOnlyList<BookingFlightPassengerEntity>>;

internal sealed class GetBookingFlightPassengersHandler(IUnitOfWork uow)
    : IRequestHandler<GetBookingFlightPassengersQuery, IReadOnlyList<BookingFlightPassengerEntity>>
{
    public Task<IReadOnlyList<BookingFlightPassengerEntity>> Handle(GetBookingFlightPassengersQuery request, CancellationToken ct)
    {
        return uow.Repository<BookingFlightPassengerEntity>().GetAllAsync(ct);
    }
}