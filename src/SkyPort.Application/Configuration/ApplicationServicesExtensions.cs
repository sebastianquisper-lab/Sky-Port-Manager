using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace SkyPort.Application.Configuration;

public static class ApplicationServicesExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var applicationAssembly = typeof(ApplicationMarker).Assembly;
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(applicationAssembly));

        foreach (var rulesType in applicationAssembly.GetTypes().Where(type =>
                     type.IsClass &&
                     !type.IsAbstract &&
                     type.Name.EndsWith("Rules", StringComparison.Ordinal)))
        {
            services.AddScoped(rulesType);
        }

        return services;
    }
}
