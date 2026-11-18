namespace Sky.Application.UseCases.BaggageType.Commands;

using MediatR;
using Sky.Domain.Ports;
using BaggageTypeEntity = Sky.Domain.Entities.BaggageType;


public sealed record UpdateBaggageTypeCommand(
    int BaggageTypeId,
    string TypeName,
    decimal? MaxWeightKg) : IRequest<BaggageTypeEntity?>;

internal sealed class UpdateBaggageTypeHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateBaggageTypeCommand, BaggageTypeEntity?>
{
    public async Task<BaggageTypeEntity?> Handle(UpdateBaggageTypeCommand request, CancellationToken ct)
    {
        var baggageType = await uow.Repository<BaggageTypeEntity>().GetByIdAsync(request.BaggageTypeId, ct);
        if (baggageType is null) return null;

        baggageType.TypeName = request.TypeName;
        baggageType.MaxWeightKg = request.MaxWeightKg;

        uow.Repository<BaggageTypeEntity>().Update(baggageType);
        await uow.SaveChangesAsync(ct);

        return baggageType;
    }
}