namespace Sky.Application.UseCases.CabinClass.Commands;

using MediatR;
using Sky.Domain.Ports;
using CabinClassEntity = Sky.Domain.Entities.CabinClass;


public sealed record CreateCabinClassCommand(
    string ClassCode,
    string ClassName) : IRequest<CabinClassEntity>;

internal sealed class CreateCabinClassHandler(IUnitOfWork uow)
    : IRequestHandler<CreateCabinClassCommand, CabinClassEntity>
{
    public async Task<CabinClassEntity> Handle(CreateCabinClassCommand request, CancellationToken ct)
    {
        var cabinClass = new CabinClassEntity
        {
            ClassCode = request.ClassCode,
            ClassName = request.ClassName
        };

        await uow.Repository<CabinClassEntity>().AddAsync(cabinClass, ct);
        await uow.SaveChangesAsync(ct);

        return cabinClass;
    }
}