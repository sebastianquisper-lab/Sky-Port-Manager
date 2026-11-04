using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Dtos.Aeronaves;
using SkyPort.Domain.Ports;
namespace SkyPort.Application.UseCases.Aeronaves.Queries;
public sealed record GetAeronaveByIdQuery(int AeronaveId) : IRequest<Result<AeronaveDto>>;
public sealed class GetAeronaveByIdQueryHandler(IUnitOfWork work) : IRequestHandler<GetAeronaveByIdQuery, Result<AeronaveDto>>
{
    public async Task<Result<AeronaveDto>> Handle(GetAeronaveByIdQuery request, CancellationToken cancellationToken)
    {
        var row = await work.Aeronaves.GetByIdAsync(request.AeronaveId, cancellationToken);
        return row is null ? Result<AeronaveDto>.Failure(Error.NotFound("Aeronave no encontrada."))
            : Result<AeronaveDto>.Success(new(row.AeronaveId, row.AerolineaId, row.Matricula, row.Modelo, row.Capacidad, row.Estado));
    }
}
