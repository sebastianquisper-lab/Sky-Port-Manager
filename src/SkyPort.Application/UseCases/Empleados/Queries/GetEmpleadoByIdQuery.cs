using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Dtos.Empleados;
using SkyPort.Domain.Ports;

namespace SkyPort.Application.UseCases.Empleados.Queries;

public sealed record GetEmpleadoByIdQuery(int EmpleadoId) : IRequest<Result<EmpleadoDto>>;

public sealed class GetEmpleadoByIdQueryHandler(IUnitOfWork work)
    : IRequestHandler<GetEmpleadoByIdQuery, Result<EmpleadoDto>>
{
    public async Task<Result<EmpleadoDto>> Handle(GetEmpleadoByIdQuery request, CancellationToken cancellationToken)
    {
        var employee = await work.Empleados.GetByIdAsync(request.EmpleadoId, cancellationToken);
        return employee is null
            ? Result<EmpleadoDto>.Failure(Error.NotFound("Empleado no encontrado."))
            : Result<EmpleadoDto>.Success(new EmpleadoDto(
                employee.EmpleadoId,
                employee.NumeroDocumento,
                employee.Nombres,
                employee.Apellidos,
                employee.Cargo,
                employee.Email,
                employee.Activo));
    }
}
