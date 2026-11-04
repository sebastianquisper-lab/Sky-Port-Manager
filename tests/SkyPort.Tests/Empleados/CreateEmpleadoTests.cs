using System.Linq.Expressions;
using NSubstitute;
using SkyPort.Application.Common;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.UseCases.Empleados.Commands;
using SkyPort.Domain.Entities;
using SkyPort.Domain.Exceptions;
using SkyPort.Domain.Ports;

namespace SkyPort.Tests.Empleados;

public sealed class CreateEmpleadoTests
{
    [Theory]
    [InlineData("not-an-email", Cargo.Piloto)]
    [InlineData("", "cargo-invalido")]
    public async Task RejectsInvalidEmailOrCargo(string email, string cargo)
    {
        var work = Substitute.For<IUnitOfWork>();

        var result = await new CreateEmpleadoCommandHandler(work)
            .Handle(new("D1", "Ana", "Rios", cargo, email, true), CancellationToken.None);

        Assert.Equal(ErrorType.Validation, result.Error?.Type);
        await work.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreatesTrimmedEmployeeWithOptionalEmailAndExplicitActivity()
    {
        var work = Substitute.For<IUnitOfWork>();
        var employees = Substitute.For<IGenericRepository<Empleado>>();
        work.Empleados.Returns(employees);
        employees.FirstOrDefaultAsync(Arg.Any<Expression<Func<Empleado, bool>>>(), Arg.Any<CancellationToken>()).Returns((Empleado?)null);
        employees.AddAsync(Arg.Any<Empleado>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        work.CompleteAsync(Arg.Any<CancellationToken>()).Returns(0);

        var result = await new CreateEmpleadoCommandHandler(work)
            .Handle(new(" D-1 ", " Ana ", " Rios ", Cargo.PersonalTierra, " ", false), CancellationToken.None);

        Assert.True(result.IsSuccess);
        await employees.Received(1).AddAsync(Arg.Is<Empleado>(employee =>
            employee.NumeroDocumento == "D-1" && employee.Nombres == "Ana" && employee.Apellidos == "Rios" &&
            employee.Email == null && employee.Activo == false), Arg.Any<CancellationToken>());
        await work.Received(1).CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DuplicateDocumentIsConflictAndUniqueRaceIsMapped()
    {
        var work = Substitute.For<IUnitOfWork>();
        var employees = Substitute.For<IGenericRepository<Empleado>>();
        work.Empleados.Returns(employees);
        employees.FirstOrDefaultAsync(Arg.Any<Expression<Func<Empleado, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(new Empleado { NumeroDocumento = "D1" });

        var duplicate = await new CreateEmpleadoCommandHandler(work)
            .Handle(new("D1", "Ana", "Rios", Cargo.Piloto, null, true), CancellationToken.None);
        Assert.Equal(ErrorType.Conflict, duplicate.Error?.Type);

        employees.FirstOrDefaultAsync(Arg.Any<Expression<Func<Empleado, bool>>>(), Arg.Any<CancellationToken>()).Returns((Empleado?)null);
        employees.AddAsync(Arg.Any<Empleado>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        work.CompleteAsync(Arg.Any<CancellationToken>()).Returns<Task<int>>(_ => throw new DuplicateKeyException("duplicate"));
        var race = await new CreateEmpleadoCommandHandler(work)
            .Handle(new("D1", "Ana", "Rios", Cargo.Piloto, null, true), CancellationToken.None);

        Assert.Equal(ErrorType.Conflict, race.Error?.Type);
    }
}
