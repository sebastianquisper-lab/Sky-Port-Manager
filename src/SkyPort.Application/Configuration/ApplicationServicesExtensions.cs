using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace SkyPort.Application.Configuration;

public static class ApplicationServicesExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(typeof(ApplicationMarker).Assembly));

        return services;
    }
}
