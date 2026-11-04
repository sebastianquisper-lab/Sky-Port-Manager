using NSubstitute;
using SkyPort.Application.Common;
using SkyPort.Application.Ports;
using SkyPort.Application.UseCases.Usuarios.Commands;

namespace SkyPort.Tests.Usuarios;

public sealed class UpdateUsuarioCommandTests
{
    [Fact]
    public async Task Handle_TrimsEditableProfileFields()
    {
        var service = Substitute.For<IUsuarioIdentityService>();
        service.UpdateProfileAsync("identity-key", "user@example.test", "user", Arg.Any<CancellationToken>())
            .Returns(Result.Success());
        var handler = new UpdateUsuarioCommandHandler(service);

        var result = await handler.Handle(
            new UpdateUsuarioCommand("identity-key", " user@example.test ", " user "),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        await service.Received(1).UpdateProfileAsync("identity-key", "user@example.test", "user", Arg.Any<CancellationToken>());
    }
}
