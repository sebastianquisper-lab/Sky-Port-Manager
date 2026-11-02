using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SkyPort.Application.Common.Constants;
using SkyPort.Persistence.Identity;

namespace SkyPort.Infrastructure.Services;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var services = serviceProvider;
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var configuration = services.GetRequiredService<IConfiguration>();
        var logger = services
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("SkyPort.Infrastructure.Services.IdentitySeeder");

        var roleNames = new[]
        {
            Roles.Administrador,
            Roles.OperadorVuelos,
            Roles.AgenteCheckIn
        };

        foreach (var roleName in roleNames)
        {
            if (await roleManager.RoleExistsAsync(roleName))
            {
                continue;
            }

            var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
            if (!roleResult.Succeeded)
            {
                logger.LogError("Could not create the Identity role {RoleName}.", roleName);
                throw new InvalidOperationException($"Could not create the Identity role '{roleName}'.");
            }
        }

        var adminEmail = configuration["AdminSeed:Email"]?.Trim();
        var adminPassword = configuration["AdminSeed:Password"];
        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
        {
            logger.LogWarning(
                "Initial administrator was not created because AdminSeed:Email or AdminSeed:Password is missing.");
            return;
        }

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser is null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var userResult = await userManager.CreateAsync(adminUser, adminPassword);
            if (!userResult.Succeeded)
            {
                var errorCodes = string.Join(", ", userResult.Errors.Select(error => error.Code));
                logger.LogWarning(
                    "Initial administrator could not be created. Identity validation errors: {ErrorCodes}.",
                    errorCodes);
                return;
            }

            logger.LogInformation("Initial administrator account was created.");
        }

        if (!await userManager.IsInRoleAsync(adminUser, Roles.Administrador))
        {
            var roleResult = await userManager.AddToRoleAsync(adminUser, Roles.Administrador);
            if (!roleResult.Succeeded)
            {
                var errorCodes = string.Join(", ", roleResult.Errors.Select(error => error.Code));
                logger.LogWarning(
                    "Initial administrator could not be assigned the administrator role. Identity errors: {ErrorCodes}.",
                    errorCodes);
                return;
            }
        }

        logger.LogInformation("Initial administrator has the administrator role.");
    }
}
