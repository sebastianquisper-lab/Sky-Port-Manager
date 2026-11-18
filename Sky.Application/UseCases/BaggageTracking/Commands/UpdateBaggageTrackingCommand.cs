namespace Sky.Application.UseCases.BaggageTracking.Commands;

using MediatR;
using Sky.Domain.Ports;
using BaggageTrackingEntity = Sky.Domain.Entities.BaggageTracking;


public sealed record UpdateBaggageTrackingCommand(
    int TrackingId,
    int BaggageId,
    int BaggageStatusId,
    int? AirportId,
    int? EmployeeId,
    string? LocationDescription) : IRequest<BaggageTrackingEntity?>;

internal sealed class UpdateBaggageTrackingHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateBaggageTrackingCommand, BaggageTrackingEntity?>
{
    public async Task<BaggageTrackingEntity?> Handle(UpdateBaggageTrackingCommand request, CancellationToken ct)
    {
        var baggageTracking = await uow.Repository<BaggageTrackingEntity>().GetByIdAsync(request.TrackingId, ct);
        if (baggageTracking is null) return null;

        baggageTracking.BaggageId = request.BaggageId;
        baggageTracking.BaggageStatusId = request.BaggageStatusId;
        baggageTracking.AirportId = request.AirportId;
        baggageTracking.EmployeeId = request.EmployeeId;
        baggageTracking.LocationDescription = request.LocationDescription;

        uow.Repository<BaggageTrackingEntity>().Update(baggageTracking);
        await uow.SaveChangesAsync(ct);

        return baggageTracking;
    }
}