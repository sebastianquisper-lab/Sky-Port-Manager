namespace Sky.Application.UseCases.BaggageStatus.Commands;

using MediatR;
using Sky.Domain.Ports;
using BaggageStatusEntity = Sky.Domain.Entities.BaggageStatus;


public sealed record DeleteBaggageStatusCommand(int BaggageStatusId) : IRequest<bool>;

internal sealed class DeleteBaggageStatusHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteBaggageStatusCommand, bool>
{
    public async Task<bool> Handle(DeleteBaggageStatusCommand request, CancellationToken ct)
    {
        var baggageStatus = await uow.Repository<BaggageStatusEntity>().GetByIdAsync(request.BaggageStatusId, ct);
        if (baggageStatus is null) return false;

        uow.Repository<BaggageStatusEntity>().Delete(baggageStatus);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}