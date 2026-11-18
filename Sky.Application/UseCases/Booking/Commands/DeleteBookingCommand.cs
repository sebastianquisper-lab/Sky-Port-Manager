namespace Sky.Application.UseCases.Booking.Commands;

using MediatR;
using Sky.Domain.Ports;
using BookingEntity = Sky.Domain.Entities.Booking;


public sealed record DeleteBookingCommand(int BookingId) : IRequest<bool>;

internal sealed class DeleteBookingHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteBookingCommand, bool>
{
    public async Task<bool> Handle(DeleteBookingCommand request, CancellationToken ct)
    {
        var booking = await uow.Repository<BookingEntity>().GetByIdAsync(request.BookingId, ct);
        if (booking is null) return false;

        uow.Repository<BookingEntity>().Delete(booking);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}