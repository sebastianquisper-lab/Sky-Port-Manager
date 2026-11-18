namespace Sky.Application.UseCases.BookingStatus.Queries;

using MediatR;
using Sky.Domain.Ports;
using BookingStatusEntity = Sky.Domain.Entities.BookingStatus;


public sealed record GetBookingStatusByIdQuery(int BookingStatusId) : IRequest<BookingStatusEntity?>;

internal sealed class GetBookingStatusByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetBookingStatusByIdQuery, BookingStatusEntity?>
{
    public Task<BookingStatusEntity?> Handle(GetBookingStatusByIdQuery request, CancellationToken ct)
    {
        return uow.Repository<BookingStatusEntity>().GetByIdAsync(request.BookingStatusId, ct);
    }
}