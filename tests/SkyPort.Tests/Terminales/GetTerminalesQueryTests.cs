using NSubstitute;
using SkyPort.Application.UseCases.Terminales.Queries;
using SkyPort.Application.Common;
using SkyPort.Application.UseCases.Terminales.Commands;
using SkyPort.Domain.Exceptions;
using SkyPort.Domain.Entities;
using SkyPort.Domain.Ports;

namespace SkyPort.Tests.Terminales;

public sealed class GetTerminalesQueryTests
{
    [Fact]
    public async Task DeleteRejectsReferencedTerminalWithoutRemovingIt()
    {
        var terminals = Substitute.For<IGenericRepository<Terminal>>();
        var gates = Substitute.For<IGenericRepository<Puerta>>();
        var work = Substitute.For<IUnitOfWork>();
        work.Terminales.Returns(terminals);
        work.Puertas.Returns(gates);
        terminals.GetByIdAsync(3, Arg.Any<CancellationToken>()).Returns(new Terminal { TerminalId = 3, Nombre = "Sur" });
        gates.CountAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Puerta, bool>>>(), Arg.Any<CancellationToken>()).Returns(1);

        var result = await new DeleteTerminalCommandHandler(work).Handle(new(3), CancellationToken.None);

        Assert.Equal(ErrorType.Conflict, result.Error?.Type);
        terminals.DidNotReceive().Remove(Arg.Any<Terminal>());
        await work.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteMapsForeignKeyRaceToConflict()
    {
        var terminals = Substitute.For<IGenericRepository<Terminal>>();
        var gates = Substitute.For<IGenericRepository<Puerta>>();
        var work = Substitute.For<IUnitOfWork>();
        work.Terminales.Returns(terminals);
        work.Puertas.Returns(gates);
        terminals.GetByIdAsync(3, Arg.Any<CancellationToken>()).Returns(new Terminal { TerminalId = 3, Nombre = "Sur" });
        gates.CountAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Puerta, bool>>>(), Arg.Any<CancellationToken>()).Returns(0);
        work.CompleteAsync(Arg.Any<CancellationToken>()).Returns<Task<int>>(_ => throw new ReferenceConflictException(new Exception()));

        var result = await new DeleteTerminalCommandHandler(work).Handle(new(3), CancellationToken.None);

        Assert.Equal(ErrorType.Conflict, result.Error?.Type);
        terminals.Received(1).Remove(Arg.Any<Terminal>());
    }
    [Fact]
    public async Task UpdateAllowsNoChangeAndCompletesSuccessfully()
    {
        var repository = Substitute.For<IGenericRepository<Terminal>>();
        var work = Substitute.For<IUnitOfWork>();
        work.Terminales.Returns(repository);
        var terminal = new Terminal { TerminalId = 5, Nombre = "Norte" };
        repository.GetByIdAsync(5, Arg.Any<CancellationToken>()).Returns(terminal);
        repository.FirstOrDefaultAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Terminal, bool>>>(), Arg.Any<CancellationToken>())
            .Returns((Terminal?)null);
        work.CompleteAsync(Arg.Any<CancellationToken>()).Returns(0);

        var result = await new UpdateTerminalCommandHandler(work).Handle(new(5, " Norte "), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Norte", terminal.Nombre);
        await work.Received(1).CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateRejectsDuplicateAndMissingTerminal()
    {
        var repository = Substitute.For<IGenericRepository<Terminal>>();
        var work = Substitute.For<IUnitOfWork>();
        work.Terminales.Returns(repository);
        repository.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(call =>
            (int)call[0] == 8 ? null : new Terminal { TerminalId = (int)call[0], Nombre = "A" });
        repository.FirstOrDefaultAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Terminal, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(new Terminal { TerminalId = 9, Nombre = "Duplicada" });

        var handler = new UpdateTerminalCommandHandler(work);
        var duplicate = await handler.Handle(new(7, "Duplicada"), CancellationToken.None);
        var missing = await handler.Handle(new(8, "Otro nombre"), CancellationToken.None);

        Assert.Equal(ErrorType.Conflict, duplicate.Error?.Type);
        Assert.Equal(ErrorType.NotFound, missing.Error?.Type);
        await work.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
    }
    [Theory]
    [InlineData("")]
    [InlineData("     ")]
    [InlineData("123456789012345678901234567890123456789012345678901")]
    public async Task CreateRejectsBlankOrOverlongNamesWithoutSaving(string name)
    {
        var work = Substitute.For<IUnitOfWork>();

        var result = await new CreateTerminalCommandHandler(work).Handle(new(name), CancellationToken.None);

        Assert.Equal(ErrorType.Validation, result.Error?.Type);
        await work.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateTrimsNameAndMapsUniqueKeyRaceToConflict()
    {
        var repository = Substitute.For<IGenericRepository<Terminal>>();
        var work = Substitute.For<IUnitOfWork>();
        work.Terminales.Returns(repository);
        repository.FirstOrDefaultAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Terminal, bool>>>(), Arg.Any<CancellationToken>())
            .Returns((Terminal?)null);
        repository.AddAsync(Arg.Any<Terminal>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        work.CompleteAsync(Arg.Any<CancellationToken>()).Returns<Task<int>>(_ => throw new DuplicateKeyException("duplicate"));

        var result = await new CreateTerminalCommandHandler(work).Handle(new("  Norte  "), CancellationToken.None);

        Assert.Equal(ErrorType.Conflict, result.Error?.Type);
        await repository.Received(1).AddAsync(Arg.Is<Terminal>(terminal => terminal.Nombre == "Norte"), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateRejectsNameAlreadyPresentUnderDatabaseComparison()
    {
        var repository = Substitute.For<IGenericRepository<Terminal>>();
        var work = Substitute.For<IUnitOfWork>();
        work.Terminales.Returns(repository);
        repository.FirstOrDefaultAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Terminal, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(new Terminal { TerminalId = 1, Nombre = "NORTE" });

        var result = await new CreateTerminalCommandHandler(work).Handle(new("norte"), CancellationToken.None);

        Assert.Equal(ErrorType.Conflict, result.Error?.Type);
        await repository.DidNotReceive().AddAsync(Arg.Any<Terminal>(), Arg.Any<CancellationToken>());
        await work.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DetailsReturnsNotFoundForMissingTerminal()
    {
        var repository = Substitute.For<IGenericRepository<Terminal>>();
        var work = Substitute.For<IUnitOfWork>();
        work.Terminales.Returns(repository);
        repository.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns((Terminal?)null);

        var result = await new GetTerminalByIdQueryHandler(work).Handle(new(404), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error?.Type);
        await work.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DetailsReturnsTheRequestedTerminal()
    {
        var repository = Substitute.For<IGenericRepository<Terminal>>();
        var work = Substitute.For<IUnitOfWork>();
        work.Terminales.Returns(repository);
        repository.GetByIdAsync(7, Arg.Any<CancellationToken>()).Returns(new Terminal { TerminalId = 7, Nombre = "Norte" });

        var result = await new GetTerminalByIdQueryHandler(work).Handle(new(7), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Norte", result.Value?.Nombre);
    }

    [Fact]
    public async Task ReturnsEmptyListWhenThereAreNoTerminals()
    {
        var repository = Substitute.For<IGenericRepository<Terminal>>();
        var work = Substitute.For<IUnitOfWork>();
        work.Terminales.Returns(repository);
        repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(Array.Empty<Terminal>());

        var result = await new GetTerminalesQueryHandler(work).Handle(new(), CancellationToken.None);

        Assert.Empty(result);
        await work.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task OrdersByNameAndThenIdForDeterministicTies()
    {
        var repository = Substitute.For<IGenericRepository<Terminal>>();
        var work = Substitute.For<IUnitOfWork>();
        work.Terminales.Returns(repository);
        repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(new[]
        {
            new Terminal { TerminalId = 4, Nombre = "Zulu" },
            new Terminal { TerminalId = 9, Nombre = "Alfa" },
            new Terminal { TerminalId = 2, Nombre = "Alfa" }
        });

        var result = await new GetTerminalesQueryHandler(work).Handle(new(), CancellationToken.None);

        Assert.Equal(new[] { 2, 9, 4 }, result.Select(terminal => terminal.TerminalId));
    }
}
