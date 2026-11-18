namespace Sky.Application.UseCases.BaggageType.Commands;

using MediatR;
using Sky.Domain.Ports;
using BaggageTypeEntity = Sky.Domain.Entities.BaggageType;


public sealed record DeleteBaggageTypeCommand(int BaggageTypeId) : IRequest<bool>;

internal sealed class DeleteBaggageTypeHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteBaggageTypeCommand, bool>
{
    public async Task<bool> Handle(DeleteBaggageTypeCommand request, CancellationToken ct)
    {
        var baggageType = await uow.Repository<BaggageTypeEntity>().GetByIdAsync(request.BaggageTypeId, ct);
        if (baggageType is null) return false;

        uow.Repository<BaggageTypeEntity>().Delete(baggageType);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}