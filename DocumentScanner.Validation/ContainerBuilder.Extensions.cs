using DocumentScanner.Validation.Interfaces;
using DocumentScanner.Validation.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentScanner.Validation
{
    public static class ServiceCollectionExtensions
    {
        public static bool IsServiceRegistered(this IServiceCollection services, Type type)
        {
            // Check if IConfigurationService is already registered
            return services.Any(descriptor => descriptor.ServiceType == type);

        }

        public static IServiceCollection AddValidators(this IServiceCollection services, IConfiguration configuration)
        {
            var validators = new List<IValidator> {

            };

            services.AddSingleton<IList<IValidator>>(validators);
            services.AddSingleton<IValidatorFactory, ValidatorFactory>();
            return services;
        }
    }
}
