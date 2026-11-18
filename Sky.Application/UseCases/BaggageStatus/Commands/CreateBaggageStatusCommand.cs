namespace Sky.Application.UseCases.BaggageStatus.Commands;

using MediatR;
using Sky.Domain.Ports;
using BaggageStatusEntity = Sky.Domain.Entities.BaggageStatus;


public sealed record CreateBaggageStatusCommand(string StatusName) : IRequest<BaggageStatusEntity>;

internal sealed class CreateBaggageStatusHandler(IUnitOfWork uow)
    : IRequestHandler<CreateBaggageStatusCommand, BaggageStatusEntity>
{
    public async Task<BaggageStatusEntity> Handle(CreateBaggageStatusCommand request, CancellationToken ct)
    {
        var baggageStatus = new BaggageStatusEntity
        {
            StatusName = request.StatusName
        };

        await uow.Repository<BaggageStatusEntity>().AddAsync(baggageStatus, ct);
        await uow.SaveChangesAsync(ct);

        return baggageStatus;
    }
}