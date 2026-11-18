namespace Sky.Application.UseCases.BookingStatus.Commands;

using MediatR;
using Sky.Domain.Ports;
using BookingStatusEntity = Sky.Domain.Entities.BookingStatus;


public sealed record CreateBookingStatusCommand(
    string StatusName,
    string? Description) : IRequest<BookingStatusEntity>;

internal sealed class CreateBookingStatusHandler(IUnitOfWork uow)
    : IRequestHandler<CreateBookingStatusCommand, BookingStatusEntity>
{
    public async Task<BookingStatusEntity> Handle(CreateBookingStatusCommand request, CancellationToken ct)
    {
        var bookingStatus = new BookingStatusEntity
        {
            StatusName = request.StatusName,
            Description = request.Description
        };

        await uow.Repository<BookingStatusEntity>().AddAsync(bookingStatus, ct);
        await uow.SaveChangesAsync(ct);

        return bookingStatus;
    }
}