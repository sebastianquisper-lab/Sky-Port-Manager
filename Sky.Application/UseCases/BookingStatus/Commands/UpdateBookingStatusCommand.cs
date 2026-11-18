namespace Sky.Application.UseCases.BookingStatus.Commands;

using MediatR;
using Sky.Domain.Ports;
using BookingStatusEntity = Sky.Domain.Entities.BookingStatus;


public sealed record UpdateBookingStatusCommand(
    int BookingStatusId,
    string StatusName,
    string? Description) : IRequest<BookingStatusEntity?>;

internal sealed class UpdateBookingStatusHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateBookingStatusCommand, BookingStatusEntity?>
{
    public async Task<BookingStatusEntity?> Handle(UpdateBookingStatusCommand request, CancellationToken ct)
    {
        var bookingStatus = await uow.Repository<BookingStatusEntity>().GetByIdAsync(request.BookingStatusId, ct);
        if (bookingStatus is null) return null;

        bookingStatus.StatusName = request.StatusName;
        bookingStatus.Description = request.Description;

        uow.Repository<BookingStatusEntity>().Update(bookingStatus);
        await uow.SaveChangesAsync(ct);

        return bookingStatus;
    }
}