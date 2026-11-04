using NSubstitute;
using SkyPort.Application.Common;
using SkyPort.Application.UseCases.TripulacionVuelo.Commands;
using SkyPort.Domain.Entities;
using SkyPort.Domain.Exceptions;
using SkyPort.Domain.Ports;
namespace SkyPort.Tests.Tripulacion;
public sealed class AssignmentCreationTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task DuplicateCombinationReturnsConflict(bool preexisting)
    {
        var work = Substitute.For<IUnitOfWork>();
        var flights = Substitute.For<IGenericRepository<Vuelo>>();
        var employees = Substitute.For<IGenericRepository<Empleado>>();
        var assignments = Substitute.For<IGenericRepository<Tripulacionvuelo>>();
        work.Vuelos.Returns(flights); work.Empleados.Returns(employees); work.TripulacionesVuelo.Returns(assignments);
        flights.GetByIdAsync(7, Arg.Any<CancellationToken>()).Returns(new Vuelo());
        employees.GetByIdAsync(12, Arg.Any<CancellationToken>()).Returns(new Empleado());
        if (preexisting) assignments.GetByKeysAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>()).Returns(new Tripulacionvuelo());
        else work.CompleteAsync(Arg.Any<CancellationToken>()).Returns(_ => Task.FromException<int>(new DuplicateKeyException("duplicate")));
        var result = await new CreateTripulacionVueloCommandHandler(work).Handle(new(7, 12, "Piloto"), default);
        Assert.Equal(ErrorType.Conflict, result.Error?.Type);
        if (preexisting) await work.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
    }
}
