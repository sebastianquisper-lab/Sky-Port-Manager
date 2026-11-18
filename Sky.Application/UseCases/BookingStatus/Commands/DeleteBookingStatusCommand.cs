namespace Sky.Application.UseCases.BookingStatus.Commands;

using MediatR;
using Sky.Domain.Ports;
using BookingStatusEntity = Sky.Domain.Entities.BookingStatus;


public sealed record DeleteBookingStatusCommand(int BookingStatusId) : IRequest<bool>;

internal sealed class DeleteBookingStatusHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteBookingStatusCommand, bool>
{
    public async Task<bool> Handle(DeleteBookingStatusCommand request, CancellationToken ct)
    {
        var bookingStatus = await uow.Repository<BookingStatusEntity>().GetByIdAsync(request.BookingStatusId, ct);
        if (bookingStatus is null) return false;

        uow.Repository<BookingStatusEntity>().Delete(bookingStatus);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}