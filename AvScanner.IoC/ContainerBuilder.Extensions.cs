using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AvScanner.Validation;
using Common.Util.Services;
using Microsoft.ApplicationInsights;

namespace AvScanner.IoC
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
            services.AddLogging(builder =>
            {
                builder.AddAzureWebAppDiagnostics();
                builder.AddConsole(); // Configure your desired logging provider
                var telemetryClient = services.BuildServiceProvider().GetService<TelemetryClient>();
                if (telemetryClient != null)
                {
                    builder.AddProvider(new AppInsightsLoggerProvider(telemetryClient));
                }
            });

            services.AddValidators(configuration);

            // Register services for the custom module.
            Container.RegisterServices(services, configuration);

            return services;
        }

        // You can add more extension methods here for other modules.
    }
}
