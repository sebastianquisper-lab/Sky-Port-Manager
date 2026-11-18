namespace Sky.Application.UseCases.Booking.Queries;

using MediatR;
using Sky.Domain.Ports;
using BookingEntity = Sky.Domain.Entities.Booking;


public sealed record GetBookingByIdQuery(int BookingId) : IRequest<BookingEntity?>;

internal sealed class GetBookingByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetBookingByIdQuery, BookingEntity?>
{
    public Task<BookingEntity?> Handle(GetBookingByIdQuery request, CancellationToken ct)
    {
        return uow.Repository<BookingEntity>().GetByIdAsync(request.BookingId, ct);
    }
}