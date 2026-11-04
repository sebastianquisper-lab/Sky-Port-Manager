using MediatR;
using SkyPort.Application.Dtos.Empleados;
using SkyPort.Domain.Ports;

namespace SkyPort.Application.UseCases.Empleados.Queries;

public sealed record GetEmpleadosQuery : IRequest<IReadOnlyList<EmpleadoDto>>;

public sealed class GetEmpleadosQueryHandler(IUnitOfWork work)
    : IRequestHandler<GetEmpleadosQuery, IReadOnlyList<EmpleadoDto>>
{
    public async Task<IReadOnlyList<EmpleadoDto>> Handle(GetEmpleadosQuery request, CancellationToken cancellationToken)
    {
        var employees = await work.Empleados.GetAllAsync(cancellationToken);
        return employees
            .OrderBy(employee => employee.Apellidos)
            .ThenBy(employee => employee.Nombres)
            .ThenBy(employee => employee.EmpleadoId)
            .Select(employee => new EmpleadoDto(
                employee.EmpleadoId,
                employee.NumeroDocumento,
                employee.Nombres,
                employee.Apellidos,
                employee.Cargo,
                employee.Email,
                employee.Activo))
            .ToList();
    }
}
