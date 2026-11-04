using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Common.Constants;
using SkyPort.Domain.Entities;
using SkyPort.Domain.Exceptions;
using SkyPort.Domain.Ports;
namespace SkyPort.Application.UseCases.Aeronaves.Commands;
public sealed record CreateAeronaveCommand(int AerolineaId, string Matricula, string Modelo, int Capacidad, string Estado) : IRequest<Result<int>>;
public sealed class CreateAeronaveCommandHandler(IUnitOfWork work) : IRequestHandler<CreateAeronaveCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateAeronaveCommand request, CancellationToken cancellationToken)
    {
        var matricula = request.Matricula?.Trim() ?? "";
        var modelo = request.Modelo?.Trim() ?? "";
        if (matricula.Length is < 1 or > 20 || modelo.Length is < 1 or > 60 ||
            request.Capacidad is < 1 or > 900 ||
            request.Estado is not (EstadoAeronave.Operativa or EstadoAeronave.Mantenimiento or EstadoAeronave.FueraDeServicio))
            return Result<int>.Failure(Error.Validation("Datos de aeronave invalidos."));
        if (await work.Aerolineas.GetByIdAsync(request.AerolineaId, cancellationToken) is null)
            return Result<int>.Failure(Error.Validation("La aerolinea no existe."));
        if (await work.Aeronaves.FirstOrDefaultAsync(x => x.Matricula == matricula, cancellationToken) is not null)
            return Result<int>.Failure(Error.Conflict("La matricula ya existe."));
        var row = new Aeronave { AerolineaId = request.AerolineaId, Matricula = matricula, Modelo = modelo, Capacidad = request.Capacidad, Estado = request.Estado };
        await work.Aeronaves.AddAsync(row, cancellationToken);
        try { await work.CompleteAsync(cancellationToken); }
        catch (DuplicateKeyException) { return Result<int>.Failure(Error.Conflict("La matricula ya existe.")); }
        catch (ReferenceConflictException) { return Result<int>.Failure(Error.Conflict("La aerolinea ya no esta disponible.")); }
        return Result<int>.Success(row.AeronaveId);
    }
}
