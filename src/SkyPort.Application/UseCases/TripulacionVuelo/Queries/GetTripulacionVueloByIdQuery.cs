using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Dtos.TripulacionVuelo;
using SkyPort.Domain.Ports;
namespace SkyPort.Application.UseCases.TripulacionVuelo.Queries;
public sealed record GetTripulacionVueloByIdQuery(int VueloId, int EmpleadoId) : IRequest<Result<TripulacionVueloDto>>;
public sealed class GetTripulacionVueloByIdQueryHandler(IUnitOfWork work) : IRequestHandler<GetTripulacionVueloByIdQuery, Result<TripulacionVueloDto>>
{
    public async Task<Result<TripulacionVueloDto>> Handle(GetTripulacionVueloByIdQuery request, CancellationToken cancellationToken)
    {
        var row = await work.TripulacionesVuelo.GetByKeysAsync([request.VueloId, request.EmpleadoId], cancellationToken);
        return row is null ? Result<TripulacionVueloDto>.Failure(Error.NotFound("Asignacion no encontrada."))
            : Result<TripulacionVueloDto>.Success(new(row.VueloId, row.EmpleadoId, row.RolAbordo));
    }
}
