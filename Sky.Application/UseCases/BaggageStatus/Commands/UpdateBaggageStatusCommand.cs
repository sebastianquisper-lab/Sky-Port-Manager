namespace Sky.Application.UseCases.BaggageStatus.Commands;

using MediatR;
using Sky.Domain.Ports;
using BaggageStatusEntity = Sky.Domain.Entities.BaggageStatus;


public sealed record UpdateBaggageStatusCommand(
    int BaggageStatusId,
    string StatusName) : IRequest<BaggageStatusEntity?>;

internal sealed class UpdateBaggageStatusHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateBaggageStatusCommand, BaggageStatusEntity?>
{
    public async Task<BaggageStatusEntity?> Handle(UpdateBaggageStatusCommand request, CancellationToken ct)
    {
        var baggageStatus = await uow.Repository<BaggageStatusEntity>().GetByIdAsync(request.BaggageStatusId, ct);
        if (baggageStatus is null) return null;

        baggageStatus.StatusName = request.StatusName;

        uow.Repository<BaggageStatusEntity>().Update(baggageStatus);
        await uow.SaveChangesAsync(ct);

        return baggageStatus;
    }
}