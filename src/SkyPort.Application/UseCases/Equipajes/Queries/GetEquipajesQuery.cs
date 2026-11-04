using MediatR;
using SkyPort.Application.Dtos.Equipajes;
using SkyPort.Application.Ports;

namespace SkyPort.Application.UseCases.Equipajes.Queries;

public sealed record GetEquipajesQuery : IRequest<IReadOnlyList<EquipajeDto>>;

public sealed class GetEquipajesQueryHandler(IEquipajeQueries queries)
    : IRequestHandler<GetEquipajesQuery, IReadOnlyList<EquipajeDto>>
{
    public Task<IReadOnlyList<EquipajeDto>> Handle(GetEquipajesQuery request, CancellationToken cancellationToken) =>
        queries.GetAllAsync(cancellationToken);
}
