using DocumentScanner.Validation.Interfaces;
using DocumentScanner.Validation.Services;
using DocumentScanner.Validation.Services.ScanningService;
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
                new ScanDataValidator(),
                new ScanFileValidator(),
                new ScanMultipleFilesValidator(),
                new ScanRemoteUrlValidator(),
                new ScanMultipleRemoteUrlsValidator()                
            };

            services.AddSingleton<IList<IValidator>>(validators);
            services.AddSingleton<IValidatorFactory, ValidatorFactory>();
            return services;
        }
    }
}
