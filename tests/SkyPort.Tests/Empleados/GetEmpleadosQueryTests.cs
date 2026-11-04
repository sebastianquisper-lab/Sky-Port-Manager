using NSubstitute;
using SkyPort.Application.Common;
using SkyPort.Application.UseCases.Empleados.Queries;
using SkyPort.Domain.Entities;
using SkyPort.Domain.Ports;

namespace SkyPort.Tests.Empleados;

public sealed class GetEmpleadosQueryTests
{
    [Fact]
    public async Task DetailsReturnNotFoundForMissingEmployee()
    {
        var repository = Substitute.For<IGenericRepository<Empleado>>();
        var work = Substitute.For<IUnitOfWork>();
        work.Empleados.Returns(repository);
        repository.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns((Empleado?)null);

        var result = await new GetEmpleadoByIdQueryHandler(work).Handle(new(404), CancellationToken.None);

        Assert.Equal(ErrorType.NotFound, result.Error?.Type);
    }

    [Fact]
    public async Task DetailsReturnCargoActivityAndOptionalEmail()
    {
        var repository = Substitute.For<IGenericRepository<Empleado>>();
        var work = Substitute.For<IUnitOfWork>();
        work.Empleados.Returns(repository);
        repository.GetByIdAsync(5, Arg.Any<CancellationToken>()).Returns(new Empleado
        {
            EmpleadoId = 5, NumeroDocumento = "DOC-5", Nombres = "Ada", Apellidos = "Rios",
            Cargo = "Piloto", Email = null, Activo = false
        });

        var result = await new GetEmpleadoByIdQueryHandler(work).Handle(new(5), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Piloto", result.Value?.Cargo);
        Assert.Null(result.Value?.Email);
        Assert.False(result.Value?.Activo);
    }
    [Fact]
    public async Task ReturnsEmptyListWhenPersonnelIsEmpty()
    {
        var repository = Substitute.For<IGenericRepository<Empleado>>();
        var work = Substitute.For<IUnitOfWork>();
        work.Empleados.Returns(repository);
        repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(Array.Empty<Empleado>());

        var result = await new GetEmpleadosQueryHandler(work).Handle(new(), CancellationToken.None);

        Assert.Empty(result);
        await work.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task OrdersBySurnameNameAndIdAndPreservesRoleAndActivity()
    {
        var repository = Substitute.For<IGenericRepository<Empleado>>();
        var work = Substitute.For<IUnitOfWork>();
        work.Empleados.Returns(repository);
        repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(new[]
        {
            new Empleado { EmpleadoId = 8, NumeroDocumento = "D8", Nombres = "Ana", Apellidos = "Zulu", Cargo = "Piloto", Activo = false },
            new Empleado { EmpleadoId = 4, NumeroDocumento = "D4", Nombres = "Bea", Apellidos = "Alfa", Cargo = "Administrativo", Activo = true },
            new Empleado { EmpleadoId = 2, NumeroDocumento = "D2", Nombres = "Bea", Apellidos = "Alfa", Cargo = "Administrativo", Activo = null }
        });

        var result = await new GetEmpleadosQueryHandler(work).Handle(new(), CancellationToken.None);

        Assert.Equal(new[] { 2, 4, 8 }, result.Select(employee => employee.EmpleadoId));
        Assert.Null(result[0].Activo);
        Assert.Equal("Administrativo", result[1].Cargo);
    }
}
