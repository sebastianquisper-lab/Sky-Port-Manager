using MediatR;
using SkyPort.Application.Common;
using SkyPort.Domain.Exceptions;
using SkyPort.Domain.Ports;

namespace SkyPort.Application.UseCases.Empleados.Commands;

public sealed record DeleteEmpleadoCommand(int EmpleadoId) : IRequest<Result>;

public sealed class DeleteEmpleadoCommandHandler(IUnitOfWork work)
    : IRequestHandler<DeleteEmpleadoCommand, Result>
{
    public async Task<Result> Handle(DeleteEmpleadoCommand request, CancellationToken cancellationToken)
    {
        var employee = await work.Empleados.GetByIdAsync(request.EmpleadoId, cancellationToken);
        if (employee is null) return Result.Failure(Error.NotFound("Empleado no encontrado."));

        var hasCheckIns = await work.CheckIns.CountAsync(x => x.EmpleadoId == employee.EmpleadoId, cancellationToken) > 0;
        var hasCrewAssignments = await work.TripulacionesVuelo.CountAsync(x => x.EmpleadoId == employee.EmpleadoId, cancellationToken) > 0;
        if (hasCheckIns || hasCrewAssignments)
            return Result.Failure(Error.Conflict("No se puede eliminar un empleado con check-in o asignaciones de tripulación asociadas."));

        work.Empleados.Remove(employee);
        try
        {
            await work.CompleteAsync(cancellationToken);
        }
        catch (ReferenceConflictException)
        {
            return Result.Failure(Error.Conflict("No se puede eliminar un empleado con historial asociado."));
        }

        return Result.Success();
    }
}
