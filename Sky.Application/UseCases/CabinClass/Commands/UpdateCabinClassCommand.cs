namespace Sky.Application.UseCases.CabinClass.Commands;

using MediatR;
using Sky.Domain.Ports;
using CabinClassEntity = Sky.Domain.Entities.CabinClass;


public sealed record UpdateCabinClassCommand(
    int CabinClassId,
    string ClassCode,
    string ClassName) : IRequest<CabinClassEntity?>;

internal sealed class UpdateCabinClassHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateCabinClassCommand, CabinClassEntity?>
{
    public async Task<CabinClassEntity?> Handle(UpdateCabinClassCommand request, CancellationToken ct)
    {
        var cabinClass = await uow.Repository<CabinClassEntity>().GetByIdAsync(request.CabinClassId, ct);
        if (cabinClass is null) return null;

        cabinClass.ClassCode = request.ClassCode;
        cabinClass.ClassName = request.ClassName;

        uow.Repository<CabinClassEntity>().Update(cabinClass);
        await uow.SaveChangesAsync(ct);

        return cabinClass;
    }
}