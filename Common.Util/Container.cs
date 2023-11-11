using Common.Util.Services;
using Common.Util.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Logging;

namespace Common.Util
{
    public class Container
    {
        public static IConfigurationService ConfigurationService { get; private set; }

        public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            if (!services.IsServiceRegistered(typeof(IDateTimeProvider)))
            {
                services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            }
        }
        public static void RegisterConfigurationService(IServiceCollection services, IConfiguration configuration)
        {
            if (!services.IsServiceRegistered(typeof(ISecretsProvider)))
            {
                services.AddSingleton<ISecretsProvider, SecretsProvider>();
            }
            if (!services.IsServiceRegistered(typeof(IConfigurationService)))
            {
                services.AddSingleton<IConfigurationService, ConfigurationService>();
            }
            // services.AddSingleton<IConfiguration>(configuration);
            ConfigurationService = services.BuildServiceProvider().GetRequiredService<IConfigurationService>().WithConfiguration(configuration);

        }

    }
}
