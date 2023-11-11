using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentScanner.Validation;

namespace DocumentScanner.IoC
{
    public static class ServiceCollectionExtensions
    {
        public static bool IsServiceRegistered(this IServiceCollection services, Type type)
        {
            // Check if IConfigurationService is already registered
            return services.Any(descriptor => descriptor.ServiceType == type);

        }
        public static IServiceCollection AddScanningServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddValidators(configuration);

            // Register services for the custom module.
            Container.RegisterServices(services, configuration);

            return services;
        }

        // You can add more extension methods here for other modules.
    }
}
