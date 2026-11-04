using System.Linq.Expressions;
using NSubstitute;
using SkyPort.Application.Common;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.UseCases.Empleados.Commands;
using SkyPort.Domain.Entities;
using SkyPort.Domain.Ports;

namespace SkyPort.Tests.Empleados;

public sealed class UpdateEmpleadoTests
{
    [Fact]
    public async Task UpdateAllowsNoChangesAndDoesNotTouchHistoricalAssignments()
    {
        var work = Substitute.For<IUnitOfWork>();
        var employees = Substitute.For<IGenericRepository<Empleado>>();
        work.Empleados.Returns(employees);
        var employee = new Empleado
        {
            EmpleadoId = 4, NumeroDocumento = "D4", Nombres = "Ana", Apellidos = "Rios",
            Cargo = Cargo.Piloto, Email = null, Activo = true
        };
        employees.GetByIdAsync(4, Arg.Any<CancellationToken>()).Returns(employee);
        employees.FirstOrDefaultAsync(Arg.Any<Expression<Func<Empleado, bool>>>(), Arg.Any<CancellationToken>()).Returns((Empleado?)null);
        work.CompleteAsync(Arg.Any<CancellationToken>()).Returns(0);

        var result = await new UpdateEmpleadoCommandHandler(work)
            .Handle(new(4, "D4", "Ana", "Rios", Cargo.Piloto, null, false), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.False(employee.Activo);
    }

    [Fact]
    public async Task UpdateReportsDuplicateAndMissingEmployee()
    {
        var work = Substitute.For<IUnitOfWork>();
        var employees = Substitute.For<IGenericRepository<Empleado>>();
        work.Empleados.Returns(employees);
        employees.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(call =>
            (int)call[0] == 9 ? null : new Empleado { EmpleadoId = (int)call[0], NumeroDocumento = "D" });
        employees.FirstOrDefaultAsync(Arg.Any<Expression<Func<Empleado, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(new Empleado { EmpleadoId = 2, NumeroDocumento = "D2" });

        var handler = new UpdateEmpleadoCommandHandler(work);
        var duplicate = await handler.Handle(new(1, "D2", "Ana", "Rios", Cargo.Piloto, null, true), CancellationToken.None);
        var missing = await handler.Handle(new(9, "D9", "Ana", "Rios", Cargo.Piloto, null, true), CancellationToken.None);

        Assert.Equal(ErrorType.Conflict, duplicate.Error?.Type);
        Assert.Equal(ErrorType.NotFound, missing.Error?.Type);
        await work.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
    }
}
