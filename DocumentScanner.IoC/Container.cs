using ClamAV.Net.Client;
using DocumentScanner.Application.Interfaces;
using DocumentScanner.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DocumentScanner.IoC
{
    public static class Container
    {

        public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            if (!services.IsServiceRegistered(typeof(IScanningService)))
            {
                // Register your application's dependencies with Microsoft's DI here
                services.AddSingleton<IScanningService, ScanningService>();
            }
            if (!services.IsServiceRegistered(typeof(IClamAvClient)))
            {
                if (string.IsNullOrEmpty(configuration["ClamAv:ServerUrl"]))
                {
                    throw new Exception("ClamAv:ServerUrl is not configured");
                } 


                var hostUrl = new Uri(configuration["ClamAv:ServerUrl"] ?? string.Empty);
                var clamClient = ClamAvClient.Create(hostUrl);
                services.AddSingleton<IClamAvClient>(clamClient);
            }
        }
    }
}