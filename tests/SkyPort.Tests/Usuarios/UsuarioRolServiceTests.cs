using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using SkyPort.Application.Common.Constants;
using SkyPort.Infrastructure.Services;
using SkyPort.Persistence.Identity;

namespace SkyPort.Tests.Usuarios;

public sealed class UsuarioRolServiceTests
{
    [Fact]
    public async Task RevokeAsync_ProtectsAdministratorMembership()
    {
        var user = new ApplicationUser { Id = "user-id" };
        var role = new IdentityRole(SkyPort.Application.Common.Constants.Roles.Administrador) { Id = "admin-role-id" };
        var (users, roles) = CreateManagers(user, role);
        var service = new UsuarioRolService(users, roles);

        var result = await service.RevokeAsync(user.Id, role.Id, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(SkyPort.Application.Common.ErrorType.Forbidden, result.Error?.Type);
        await users.DidNotReceive().RemoveFromRoleAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>());
    }

    [Fact]
    public async Task AssignAsync_RejectsExistingMembership()
    {
        var user = new ApplicationUser { Id = "user-id" };
        var role = new IdentityRole("Auditor") { Id = "role-id" };
        var (users, roles) = CreateManagers(user, role);
        users.IsInRoleAsync(user, role.Name!).Returns(true);
        var service = new UsuarioRolService(users, roles);

        var result = await service.AssignAsync(user.Id, role.Id, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(SkyPort.Application.Common.ErrorType.Conflict, result.Error?.Type);
        await users.DidNotReceive().AddToRoleAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>());
    }

    private static (UserManager<ApplicationUser> Users, RoleManager<IdentityRole> Roles) CreateManagers(
        ApplicationUser user,
        IdentityRole role)
    {
        var userStore = Substitute.For<IUserStore<ApplicationUser>>();
        var users = Substitute.For<UserManager<ApplicationUser>>(
            userStore,
            Options.Create(new IdentityOptions()),
            Substitute.For<IPasswordHasher<ApplicationUser>>(),
            Array.Empty<IUserValidator<ApplicationUser>>(),
            Array.Empty<IPasswordValidator<ApplicationUser>>(),
            Substitute.For<ILookupNormalizer>(),
            new IdentityErrorDescriber(),
            Substitute.For<IServiceProvider>(),
            NullLogger<UserManager<ApplicationUser>>.Instance);
        users.FindByIdAsync(user.Id).Returns(user);

        var roleStore = Substitute.For<IRoleStore<IdentityRole>>();
        var roles = Substitute.For<RoleManager<IdentityRole>>(
            roleStore,
            Array.Empty<IRoleValidator<IdentityRole>>(),
            Substitute.For<ILookupNormalizer>(),
            new IdentityErrorDescriber(),
            NullLogger<RoleManager<IdentityRole>>.Instance);
        roles.FindByIdAsync(role.Id).Returns(role);
        return (users, roles);
    }
}
