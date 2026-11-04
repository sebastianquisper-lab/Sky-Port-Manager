using System.Linq.Expressions;
using NSubstitute;
using SkyPort.Application.Common;
using SkyPort.Application.UseCases.Empleados.Commands;
using SkyPort.Domain.Entities;
using SkyPort.Domain.Exceptions;
using SkyPort.Domain.Ports;

namespace SkyPort.Tests.Empleados;

public sealed class DeleteEmpleadoTests
{
    [Fact]
    public async Task DeleteRejectsCheckInAndCrewHistory()
    {
        foreach (var (checkInCount, crewCount) in new[] { (1, 0), (0, 1) })
        {
            var work = Substitute.For<IUnitOfWork>();
            var employees = Substitute.For<IGenericRepository<Empleado>>();
            var checkIns = Substitute.For<IGenericRepository<Checkin>>();
            var crew = Substitute.For<IGenericRepository<Tripulacionvuelo>>();
            work.Empleados.Returns(employees);
            work.CheckIns.Returns(checkIns);
            work.TripulacionesVuelo.Returns(crew);
            employees.GetByIdAsync(12, Arg.Any<CancellationToken>()).Returns(new Empleado { EmpleadoId = 12 });
            checkIns.CountAsync(Arg.Any<Expression<Func<Checkin, bool>>>(), Arg.Any<CancellationToken>()).Returns(checkInCount);
            crew.CountAsync(Arg.Any<Expression<Func<Tripulacionvuelo, bool>>>(), Arg.Any<CancellationToken>()).Returns(crewCount);

            var result = await new DeleteEmpleadoCommandHandler(work).Handle(new(12), CancellationToken.None);

            Assert.Equal(ErrorType.Conflict, result.Error?.Type);
            employees.DidNotReceive().Remove(Arg.Any<Empleado>());
            await work.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
        }
    }

    [Fact]
    public async Task DeleteRemovesEmployeeWithoutReferencesAndZeroChangesAreSuccessful()
    {
        var work = Substitute.For<IUnitOfWork>();
        var employees = Substitute.For<IGenericRepository<Empleado>>();
        work.Empleados.Returns(employees);
        work.CheckIns.Returns(Substitute.For<IGenericRepository<Checkin>>());
        work.TripulacionesVuelo.Returns(Substitute.For<IGenericRepository<Tripulacionvuelo>>());
        var employee = new Empleado { EmpleadoId = 12 };
        employees.GetByIdAsync(12, Arg.Any<CancellationToken>()).Returns(employee);
        work.CheckIns.CountAsync(Arg.Any<Expression<Func<Checkin, bool>>>(), Arg.Any<CancellationToken>()).Returns(0);
        work.TripulacionesVuelo.CountAsync(Arg.Any<Expression<Func<Tripulacionvuelo, bool>>>(), Arg.Any<CancellationToken>()).Returns(0);
        work.CompleteAsync(Arg.Any<CancellationToken>()).Returns(0);

        var result = await new DeleteEmpleadoCommandHandler(work).Handle(new(12), CancellationToken.None);

        Assert.True(result.IsSuccess);
        employees.Received(1).Remove(employee);
    }

    [Fact]
    public async Task DeleteMapsForeignKeyRaceToConflict()
    {
        var work = Substitute.For<IUnitOfWork>();
        var employees = Substitute.For<IGenericRepository<Empleado>>();
        work.Empleados.Returns(employees);
        work.CheckIns.Returns(Substitute.For<IGenericRepository<Checkin>>());
        work.TripulacionesVuelo.Returns(Substitute.For<IGenericRepository<Tripulacionvuelo>>());
        employees.GetByIdAsync(12, Arg.Any<CancellationToken>()).Returns(new Empleado { EmpleadoId = 12 });
        work.CheckIns.CountAsync(Arg.Any<Expression<Func<Checkin, bool>>>(), Arg.Any<CancellationToken>()).Returns(0);
        work.TripulacionesVuelo.CountAsync(Arg.Any<Expression<Func<Tripulacionvuelo, bool>>>(), Arg.Any<CancellationToken>()).Returns(0);
        work.CompleteAsync(Arg.Any<CancellationToken>()).Returns<Task<int>>(_ => throw new ReferenceConflictException(new Exception()));

        var result = await new DeleteEmpleadoCommandHandler(work).Handle(new(12), CancellationToken.None);

        Assert.Equal(ErrorType.Conflict, result.Error?.Type);
    }
}
