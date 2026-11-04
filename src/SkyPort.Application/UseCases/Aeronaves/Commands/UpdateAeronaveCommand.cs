using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Common.Constants;
using SkyPort.Domain.Exceptions;
using SkyPort.Domain.Ports;
namespace SkyPort.Application.UseCases.Aeronaves.Commands;
public sealed record UpdateAeronaveCommand(int AeronaveId, int AerolineaId, string Matricula, string Modelo, int Capacidad, string Estado) : IRequest<Result>;
public sealed class UpdateAeronaveCommandHandler(IUnitOfWork work) : IRequestHandler<UpdateAeronaveCommand, Result>
{
    public async Task<Result> Handle(UpdateAeronaveCommand request, CancellationToken cancellationToken)
    {
        var row = await work.Aeronaves.GetByIdAsync(request.AeronaveId, cancellationToken);
        if (row is null) return Result.Failure(Error.NotFound("Aeronave no encontrada."));
        var matricula = request.Matricula?.Trim() ?? "";
        var modelo = request.Modelo?.Trim() ?? "";
        if (matricula.Length is < 1 or > 20 || modelo.Length is < 1 or > 60 || request.Capacidad is < 1 or > 900 ||
            request.Estado is not (EstadoAeronave.Operativa or EstadoAeronave.Mantenimiento or EstadoAeronave.FueraDeServicio))
            return Result.Failure(Error.Validation("Datos de aeronave invalidos."));
        if (await work.Aerolineas.GetByIdAsync(request.AerolineaId, cancellationToken) is null)
            return Result.Failure(Error.Validation("La aerolinea no existe."));
        if (row.Capacidad != request.Capacidad &&
            await work.Vuelos.CountAsync(x => x.AeronaveId == row.AeronaveId, cancellationToken) > 0)
            return Result.Failure(Error.Conflict("Cambio de capacidad con vuelos asociados pendiente de politica del equipo."));
        if (await work.Aeronaves.FirstOrDefaultAsync(x => x.AeronaveId != row.AeronaveId && x.Matricula == matricula, cancellationToken) is not null)
            return Result.Failure(Error.Conflict("La matricula ya existe."));
        row.AerolineaId = request.AerolineaId;
        row.Matricula = matricula;
        row.Modelo = modelo;
        row.Capacidad = request.Capacidad;
        row.Estado = request.Estado;
        try { await work.CompleteAsync(cancellationToken); }
        catch (DuplicateKeyException) { return Result.Failure(Error.Conflict("La matricula ya existe.")); }
        catch (ReferenceConflictException) { return Result.Failure(Error.Conflict("La aerolinea ya no esta disponible.")); }
        return Result.Success();
    }
}
