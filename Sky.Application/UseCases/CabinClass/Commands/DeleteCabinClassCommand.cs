namespace Sky.Application.UseCases.CabinClass.Commands;

using MediatR;
using Sky.Domain.Ports;
using CabinClassEntity = Sky.Domain.Entities.CabinClass;


public sealed record DeleteCabinClassCommand(int CabinClassId) : IRequest<bool>;

internal sealed class DeleteCabinClassHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteCabinClassCommand, bool>
{
    public async Task<bool> Handle(DeleteCabinClassCommand request, CancellationToken ct)
    {
        var cabinClass = await uow.Repository<CabinClassEntity>().GetByIdAsync(request.CabinClassId, ct);
        if (cabinClass is null) return false;

        uow.Repository<CabinClassEntity>().Delete(cabinClass);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}