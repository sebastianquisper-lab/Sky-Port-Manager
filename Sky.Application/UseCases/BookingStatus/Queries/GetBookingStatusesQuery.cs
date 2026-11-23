namespace Sky.Application.UseCases.BookingStatus.Queries;

using MediatR;
using Sky.Domain.Ports;
using BookingStatusEntity = Sky.Domain.Entities.BookingStatus;


public sealed record GetBookingStatusesQuery : IRequest<IReadOnlyList<BookingStatusEntity>>;

internal sealed class GetBookingStatusesHandler(IUnitOfWork uow)
    : IRequestHandler<GetBookingStatusesQuery, IReadOnlyList<BookingStatusEntity>>
{
    public Task<IReadOnlyList<BookingStatusEntity>> Handle(GetBookingStatusesQuery request, CancellationToken ct)
    {
        return uow.Repository<BookingStatusEntity>().GetAllAsync(ct);
    }
}