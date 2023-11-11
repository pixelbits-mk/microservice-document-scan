using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Util.Services
{
    public class AppInsightsLoggerProvider : ILoggerProvider
    {
        private readonly TelemetryClient _telemetryClient;

        public AppInsightsLoggerProvider(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }
        public ILogger CreateLogger(string categoryName)
        {
            return new AppInsightsLogger(_telemetryClient, categoryName);
        }

        public void Dispose() { }
    }
}
