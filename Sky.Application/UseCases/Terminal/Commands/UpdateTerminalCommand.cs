using MediatR;
using Sky.Domain.Ports;
using TerminalEntity = Sky.Domain.Entities.Terminal;

namespace Sky.Application.UseCases.Terminal.Commands;

public sealed record UpdateTerminalCommand(
    int TerminalId,
    int AirportId,
    string TerminalCode,
    string? TerminalName,
    bool IsActive) : IRequest<TerminalEntity?>;

internal sealed class UpdateTerminalHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateTerminalCommand, TerminalEntity?>
{
    public async Task<TerminalEntity?> Handle(UpdateTerminalCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<TerminalEntity>().GetByIdAsync(request.TerminalId, ct);
        if (entity is null) return null;

        entity.AirportId = request.AirportId;
        entity.TerminalCode = request.TerminalCode;
        entity.TerminalName = request.TerminalName;
        entity.IsActive = request.IsActive;

        uow.Repository<TerminalEntity>().Update(entity);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
