using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using SkyPort.Application.Common.Constants;
using SkyPort.Infrastructure.Services;
using SkyPort.Persistence.Identity;

namespace SkyPort.Tests.Usuarios;

public sealed class UsuarioIdentityServiceDeleteTests
{
    [Fact]
    public async Task DeleteAsync_RejectsCurrentAccount()
    {
        var user = new ApplicationUser { Id = "same-id" };
        var users = CreateUserManager(user);
        var service = new UsuarioIdentityService(users);

        var result = await service.DeleteAsync(user.Id, user.Id, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(SkyPort.Application.Common.ErrorType.Forbidden, result.Error?.Type);
        await users.DidNotReceive().DeleteAsync(Arg.Any<ApplicationUser>());
    }

    [Fact]
    public async Task DeleteAsync_RejectsAdministratorAccount()
    {
        var user = new ApplicationUser { Id = "admin-id" };
        var users = CreateUserManager(user);
        users.IsInRoleAsync(user, SkyPort.Application.Common.Constants.Roles.Administrador).Returns(true);
        var service = new UsuarioIdentityService(users);

        var result = await service.DeleteAsync(user.Id, "other-admin", CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(SkyPort.Application.Common.ErrorType.Forbidden, result.Error?.Type);
        await users.DidNotReceive().DeleteAsync(Arg.Any<ApplicationUser>());
    }

    private static UserManager<ApplicationUser> CreateUserManager(ApplicationUser user)
    {
        var store = Substitute.For<IUserStore<ApplicationUser>>();
        var users = Substitute.For<UserManager<ApplicationUser>>(
            store,
            Options.Create(new IdentityOptions()),
            Substitute.For<IPasswordHasher<ApplicationUser>>(),
            Array.Empty<IUserValidator<ApplicationUser>>(),
            Array.Empty<IPasswordValidator<ApplicationUser>>(),
            Substitute.For<ILookupNormalizer>(),
            new IdentityErrorDescriber(),
            Substitute.For<IServiceProvider>(),
            NullLogger<UserManager<ApplicationUser>>.Instance);
        users.FindByIdAsync(user.Id).Returns(user);
        return users;
    }
}
