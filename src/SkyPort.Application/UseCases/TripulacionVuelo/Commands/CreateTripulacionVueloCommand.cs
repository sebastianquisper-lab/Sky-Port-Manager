using MediatR;
using SkyPort.Application.Common;
using SkyPort.Domain.Entities;
using SkyPort.Domain.Exceptions;
using SkyPort.Domain.Ports;
namespace SkyPort.Application.UseCases.TripulacionVuelo.Commands;
public sealed record CreateTripulacionVueloCommand(int VueloId, int EmpleadoId, string RolAbordo) : IRequest<Result>;
public sealed class CreateTripulacionVueloCommandHandler(IUnitOfWork work) : IRequestHandler<CreateTripulacionVueloCommand, Result>
{
    public async Task<Result> Handle(CreateTripulacionVueloCommand request, CancellationToken cancellationToken)
    {
        var rol = request.RolAbordo?.Trim() ?? "";
        if (rol.Length is < 1 or > 30) return Result.Failure(Error.Validation("El rol a bordo es obligatorio y admite hasta 30 caracteres."));
        if (await work.Vuelos.GetByIdAsync(request.VueloId, cancellationToken) is null ||
            await work.Empleados.GetByIdAsync(request.EmpleadoId, cancellationToken) is null)
            return Result.Failure(Error.Validation("El vuelo o empleado no existe."));
        if (await work.TripulacionesVuelo.GetByKeysAsync([request.VueloId, request.EmpleadoId], cancellationToken) is not null)
            return Result.Failure(Error.Conflict("El empleado ya esta asignado al vuelo."));
        await work.TripulacionesVuelo.AddAsync(new Tripulacionvuelo { VueloId = request.VueloId, EmpleadoId = request.EmpleadoId, RolAbordo = rol }, cancellationToken);
        try { await work.CompleteAsync(cancellationToken); }
        catch (DuplicateKeyException) { return Result.Failure(Error.Conflict("El empleado ya esta asignado al vuelo.")); }
        catch (ReferenceConflictException) { return Result.Failure(Error.Conflict("El vuelo o empleado ya no esta disponible.")); }
        return Result.Success();
    }
}
