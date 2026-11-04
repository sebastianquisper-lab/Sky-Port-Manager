using NSubstitute;
using SkyPort.Application.Common;
using SkyPort.Application.Ports;
using SkyPort.Application.UseCases.Usuarios.Commands;

namespace SkyPort.Tests.Usuarios;

public sealed class UsuarioRoleCommandsTests
{
    [Fact]
    public async Task AssignHandler_ForwardsStringIdentityAndRoleKeys()
    {
        var service = Substitute.For<IUsuarioRolService>();
        service.AssignAsync("user-key", "role-key", Arg.Any<CancellationToken>()).Returns(Result.Success());
        var handler = new AsignarUsuarioRolCommandHandler(service);

        var result = await handler.Handle(new AsignarUsuarioRolCommand("user-key", "role-key"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        await service.Received(1).AssignAsync("user-key", "role-key", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RevokeHandler_ForwardsStringIdentityAndRoleKeys()
    {
        var service = Substitute.For<IUsuarioRolService>();
        service.RevokeAsync("user-key", "role-key", Arg.Any<CancellationToken>()).Returns(Result.Success());
        var handler = new RevocarUsuarioRolCommandHandler(service);

        var result = await handler.Handle(new RevocarUsuarioRolCommand("user-key", "role-key"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        await service.Received(1).RevokeAsync("user-key", "role-key", Arg.Any<CancellationToken>());
    }
}
