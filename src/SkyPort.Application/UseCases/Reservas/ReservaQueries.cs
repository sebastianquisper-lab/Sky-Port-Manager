using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Ports;
using SkyPort.Application.UseCases.Reservas.Dtos;

namespace SkyPort.Application.UseCases.Reservas;

public sealed record GetVuelosAbiertosQuery : IRequest<IReadOnlyList<VueloAbiertoDto>>;

public sealed record GetReservasByVueloQuery(int VueloId) : IRequest<IReadOnlyList<ReservaDto>>;

public sealed record GetReservasByPasajeroQuery(int PasajeroId) : IRequest<IReadOnlyList<ReservaDto>>;

public sealed record GetAsientosVueloQuery(int VueloId) : IRequest<Result<AsientosVueloDto>>;

public sealed class GetVuelosAbiertosQueryHandler(IReservaQueries queries)
    : IRequestHandler<GetVuelosAbiertosQuery, IReadOnlyList<VueloAbiertoDto>>
{
    public Task<IReadOnlyList<VueloAbiertoDto>> Handle(
        GetVuelosAbiertosQuery request,
        CancellationToken cancellationToken) => queries.GetVuelosAbiertosAsync(cancellationToken);
}

public sealed class GetReservasByVueloQueryHandler(IReservaQueries queries)
    : IRequestHandler<GetReservasByVueloQuery, IReadOnlyList<ReservaDto>>
{
    public Task<IReadOnlyList<ReservaDto>> Handle(
        GetReservasByVueloQuery request,
        CancellationToken cancellationToken) => queries.GetByVueloAsync(request.VueloId, cancellationToken);
}

public sealed class GetReservasByPasajeroQueryHandler(IReservaQueries queries)
    : IRequestHandler<GetReservasByPasajeroQuery, IReadOnlyList<ReservaDto>>
{
    public Task<IReadOnlyList<ReservaDto>> Handle(
        GetReservasByPasajeroQuery request,
        CancellationToken cancellationToken) => queries.GetByPasajeroAsync(request.PasajeroId, cancellationToken);
}

public sealed class GetAsientosVueloQueryHandler(IReservaQueries queries)
    : IRequestHandler<GetAsientosVueloQuery, Result<AsientosVueloDto>>
{
    public async Task<Result<AsientosVueloDto>> Handle(
        GetAsientosVueloQuery request,
        CancellationToken cancellationToken)
    {
        var asientos = await queries.GetAsientosAsync(request.VueloId, cancellationToken);
        return asientos is null
            ? Result<AsientosVueloDto>.Failure(Error.NotFound("No se encontró el vuelo."))
            : Result<AsientosVueloDto>.Success(asientos);
    }
}
