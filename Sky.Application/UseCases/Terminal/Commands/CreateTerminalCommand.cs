using MediatR;
using Sky.Domain.Ports;
using TerminalEntity = Sky.Domain.Entities.Terminal;

namespace Sky.Application.UseCases.Terminal.Commands;

public sealed record CreateTerminalCommand(
    int AirportId,
    string TerminalCode,
    string? TerminalName) : IRequest<TerminalEntity>;

internal sealed class CreateTerminalHandler(IUnitOfWork uow)
    : IRequestHandler<CreateTerminalCommand, TerminalEntity>
{
    public async Task<TerminalEntity> Handle(CreateTerminalCommand request, CancellationToken ct)
    {
        var entity = new TerminalEntity
        {
            AirportId = request.AirportId,
            TerminalCode = request.TerminalCode,
            TerminalName = request.TerminalName,
            IsActive = true
        };

        await uow.Repository<TerminalEntity>().AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
