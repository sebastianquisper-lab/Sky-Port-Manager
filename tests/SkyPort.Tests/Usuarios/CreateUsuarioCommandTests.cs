using NSubstitute;
using SkyPort.Application.Common;
using SkyPort.Application.Dtos.Usuarios;
using SkyPort.Application.Ports;
using SkyPort.Application.UseCases.Usuarios.Commands;

namespace SkyPort.Tests.Usuarios;

public sealed class CreateUsuarioCommandTests
{
    [Fact]
    public async Task Handle_TrimsEmailAndReturnsCreatedProfile()
    {
        var service = Substitute.For<IUsuarioIdentityService>();
        var created = new UsuarioDetalleDto("identity-key", "person@example.test", "person@example.test", false, Array.Empty<string>());
        service.CreateAsync("person@example.test", "Password123", Arg.Any<CancellationToken>())
            .Returns(Result<UsuarioDetalleDto>.Success(created));
        var handler = new CreateUsuarioCommandHandler(service);

        var result = await handler.Handle(
            new CreateUsuarioCommand(" person@example.test ", "Password123"),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Same(created, result.Value);
        await service.Received(1).CreateAsync("person@example.test", "Password123", Arg.Any<CancellationToken>());
    }
}
