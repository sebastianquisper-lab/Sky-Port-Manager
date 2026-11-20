using MediatR;
using Sky.Domain.Ports;
using RunwayEntity = Sky.Domain.Entities.Runway;

namespace Sky.Application.UseCases.Runway.Commands;

public sealed record DeleteRunwayCommand(
    int RunwayId) : IRequest<bool>;

internal sealed class DeleteRunwayHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteRunwayCommand, bool>
{
    public async Task<bool> Handle(DeleteRunwayCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<RunwayEntity>().GetByIdAsync(request.RunwayId, ct);
        if (entity is null) return false;

        uow.Repository<RunwayEntity>().Delete(entity);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}
