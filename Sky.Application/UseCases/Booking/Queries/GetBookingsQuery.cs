namespace Sky.Application.UseCases.Booking.Queries;

using MediatR;
using Sky.Domain.Ports;
using BookingEntity = Sky.Domain.Entities.Booking;


public sealed record GetBookingsQuery : IRequest<IReadOnlyList<BookingEntity>>;

internal sealed class GetBookingsHandler(IUnitOfWork uow)
    : IRequestHandler<GetBookingsQuery, IReadOnlyList<BookingEntity>>
{
    public Task<IReadOnlyList<BookingEntity>> Handle(GetBookingsQuery request, CancellationToken ct)
    {
        return uow.Repository<BookingEntity>().GetAllAsync(ct);
    }
}