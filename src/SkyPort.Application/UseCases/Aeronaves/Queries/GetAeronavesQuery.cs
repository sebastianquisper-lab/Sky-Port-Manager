using MediatR;
using SkyPort.Application.Dtos.Aeronaves;
using SkyPort.Domain.Ports;
namespace SkyPort.Application.UseCases.Aeronaves.Queries;
public sealed record GetAeronavesQuery : IRequest<IReadOnlyList<AeronaveDto>>;
public sealed class GetAeronavesQueryHandler(IUnitOfWork work) : IRequestHandler<GetAeronavesQuery, IReadOnlyList<AeronaveDto>>
{
    public async Task<IReadOnlyList<AeronaveDto>> Handle(GetAeronavesQuery request, CancellationToken cancellationToken)
    {
        var rows = await work.Aeronaves.GetAllAsync(cancellationToken);
        return rows.OrderBy(x => x.Matricula).Select(x => new AeronaveDto(x.AeronaveId, x.AerolineaId, x.Matricula, x.Modelo, x.Capacidad, x.Estado)).ToList();
    }
}
