namespace Sky.Application.UseCases.BaggageTracking.Commands;

using MediatR;
using Sky.Domain.Ports;
using BaggageTrackingEntity = Sky.Domain.Entities.BaggageTracking;


public sealed record CreateBaggageTrackingCommand(
    int BaggageId,
    int BaggageStatusId,
    int? AirportId,
    int? EmployeeId,
    string? LocationDescription) : IRequest<BaggageTrackingEntity>;

internal sealed class CreateBaggageTrackingHandler(IUnitOfWork uow)
    : IRequestHandler<CreateBaggageTrackingCommand, BaggageTrackingEntity>
{
    public async Task<BaggageTrackingEntity> Handle(CreateBaggageTrackingCommand request, CancellationToken ct)
    {
        var baggageTracking = new BaggageTrackingEntity
        {
            BaggageId = request.BaggageId,
            BaggageStatusId = request.BaggageStatusId,
            AirportId = request.AirportId,
            EmployeeId = request.EmployeeId,
            LocationDescription = request.LocationDescription,
            EventTimeUtc = DateTime.UtcNow
        };

        await uow.Repository<BaggageTrackingEntity>().AddAsync(baggageTracking, ct);
        await uow.SaveChangesAsync(ct);

        return baggageTracking;
    }
}