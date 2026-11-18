namespace Sky.Application.UseCases.BaggageType.Commands;

using MediatR;
using Sky.Domain.Ports;
using BaggageTypeEntity = Sky.Domain.Entities.BaggageType;


public sealed record CreateBaggageTypeCommand(
    string TypeName,
    decimal? MaxWeightKg) : IRequest<BaggageTypeEntity>;

internal sealed class CreateBaggageTypeHandler(IUnitOfWork uow)
    : IRequestHandler<CreateBaggageTypeCommand, BaggageTypeEntity>
{
    public async Task<BaggageTypeEntity> Handle(CreateBaggageTypeCommand request, CancellationToken ct)
    {
        var baggageType = new BaggageTypeEntity
        {
            TypeName = request.TypeName,
            MaxWeightKg = request.MaxWeightKg
        };

        await uow.Repository<BaggageTypeEntity>().AddAsync(baggageType, ct);
        await uow.SaveChangesAsync(ct);

        return baggageType;
    }
}