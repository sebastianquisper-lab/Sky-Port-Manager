using System.Linq.Expressions;
using NSubstitute;
using SkyPort.Application.Common;
using SkyPort.Application.UseCases.Puertas.Commands;
using SkyPort.Application.UseCases.Puertas.Queries;
using SkyPort.Domain.Entities;
using SkyPort.Domain.Ports;

namespace SkyPort.Tests.Puertas;

public sealed class CreatePuertaTests
{
    [Theory]
    [InlineData(0, "A1", true)]
    [InlineData(1, "", true)]
    [InlineData(1, "12345678901", true)]
    [InlineData(1, "A1", null)]
    public async Task RejectsInvalidInputWithoutSaving(int terminalId, string code, bool? available)
    {
        var work = Substitute.For<IUnitOfWork>();

        var result = await new CreatePuertaCommandHandler(work).Handle(new(terminalId, code, available), CancellationToken.None);

        Assert.Equal(ErrorType.Validation, result.Error?.Type);
        await work.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RejectsMissingTerminalAndDuplicateInSameTerminal()
    {
        var work = Substitute.For<IUnitOfWork>();
        var terminals = Substitute.For<IGenericRepository<Terminal>>();
        var gates = Substitute.For<IGenericRepository<Puerta>>();
        work.Terminales.Returns(terminals);
        work.Puertas.Returns(gates);
        terminals.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns((Terminal?)null);

        var handler = new CreatePuertaCommandHandler(work);
        var missing = await handler.Handle(new(1, "A1", true), CancellationToken.None);
        Assert.Equal(ErrorType.Validation, missing.Error?.Type);

        terminals.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(new Terminal { TerminalId = 1, Nombre = "Norte" });
        var existing = new Puerta { TerminalId = 1, Codigo = "A1" };
        gates.FirstOrDefaultAsync(Arg.Any<Expression<Func<Puerta, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(call => call.Arg<Expression<Func<Puerta, bool>>>().Compile()(existing) ? existing : null);

        var duplicate = await handler.Handle(new(1, "A1", true), CancellationToken.None);

        Assert.Equal(ErrorType.Conflict, duplicate.Error?.Type);
        await work.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SameCodeInDifferentTerminalIsAllowedAndAvailabilityIsExplicit()
    {
        var work = Substitute.For<IUnitOfWork>();
        var terminals = Substitute.For<IGenericRepository<Terminal>>();
        var gates = Substitute.For<IGenericRepository<Puerta>>();
        work.Terminales.Returns(terminals);
        work.Puertas.Returns(gates);
        terminals.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns(new Terminal { TerminalId = 2, Nombre = "Sur" });
        var existing = new Puerta { TerminalId = 1, Codigo = "A1" };
        gates.FirstOrDefaultAsync(Arg.Any<Expression<Func<Puerta, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(call => call.Arg<Expression<Func<Puerta, bool>>>().Compile()(existing) ? existing : null);
        gates.AddAsync(Arg.Any<Puerta>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        work.CompleteAsync(Arg.Any<CancellationToken>()).Returns(0);

        var result = await new CreatePuertaCommandHandler(work).Handle(new(2, " A1 ", false), CancellationToken.None);

        Assert.True(result.IsSuccess);
        await gates.Received(1).AddAsync(Arg.Is<Puerta>(x => x.TerminalId == 2 && x.Codigo == "A1" && x.Disponible == false), Arg.Any<CancellationToken>());
        await work.Received(1).CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task TerminalOptionsAreOrderedAndDoNotWrite()
    {
        var work = Substitute.For<IUnitOfWork>();
        var repository = Substitute.For<IGenericRepository<Terminal>>();
        work.Terminales.Returns(repository);
        repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(new[]
        {
            new Terminal { TerminalId = 2, Nombre = "Sur" },
            new Terminal { TerminalId = 1, Nombre = "Norte" }
        });

        var result = await new GetTerminalChoicesQueryHandler(work).Handle(new(), CancellationToken.None);

        Assert.Equal(new[] { "Norte", "Sur" }, result.Select(x => x.Nombre));
        await work.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
    }
}
