using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using SkyPort.Application.Common.Constants;

namespace SkyPort.Web.Configuration;

public static class SkyPortAuthorizationExtensions
{
    public static IServiceCollection AddSkyPortAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            options.AddPolicy(
                Policies.SoloAdmin,
                policy => policy.RequireRole(Roles.Administrador));

            options.AddPolicy(
                Policies.GestionOperaciones,
                policy => policy.RequireRole(Roles.Administrador, Roles.OperadorVuelos));

            options.AddPolicy(
                Policies.GestionPasajeros,
                policy => policy.RequireRole(Roles.Administrador, Roles.AgenteCheckIn));

            options.AddPolicy(
                Policies.TodosLosRoles,
                policy => policy.RequireRole(
                    Roles.Administrador,
                    Roles.OperadorVuelos,
                    Roles.AgenteCheckIn));
        });

        return services;
    }
}
