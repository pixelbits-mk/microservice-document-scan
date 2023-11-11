using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Common.Util;
using Common.Util.Interfaces;
using Common.Util.Services;

namespace Common.Util
{
    public static class ServiceCollectionExtensions
    {
        public static bool IsServiceRegistered(this IServiceCollection services, Type type)
        {
            // Check if IConfigurationService is already registered
            return services.Any(descriptor => descriptor.ServiceType == type);

        }
        public static IServiceCollection AddUtilServices(this IServiceCollection services, IConfiguration configuration)
        {
            Container.RegisterServices(services, configuration);

            return services;
        }

        public static IServiceCollection AddConfigurationService(this IServiceCollection services, IConfiguration configuration)
        {
            Container.RegisterConfigurationService( services, configuration);
            services.AddSingleton<IConfigurationService>(Common.Util.Container.ConfigurationService);
            return services;
        }

        // You can add more extension methods here for other services.
    }
}
