namespace Sky.Application.UseCases.BookingStatus.Queries;

using MediatR;
using Sky.Domain.Ports;
using BookingStatusEntity = Sky.Domain.Entities.BookingStatus;


public sealed record GetBookingStatusesQuery : IRequest<List<BookingStatusEntity>>;

internal sealed class GetBookingStatusesHandler(IUnitOfWork uow)
    : IRequestHandler<GetBookingStatusesQuery, List<BookingStatusEntity>>
{
    public Task<List<BookingStatusEntity>> Handle(GetBookingStatusesQuery request, CancellationToken ct)
    {
        return uow.Repository<BookingStatusEntity>().GetAllAsync(ct);
    }
}