using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Common.Util.Services
{
    public class AppInsightsLogger : ILogger
    {
        private readonly TelemetryClient _telemetryClient;
        private readonly string _categoryName;
        private readonly Dictionary<LogLevel, SeverityLevel> _severityLevels;

        public AppInsightsLogger(TelemetryClient telemetryClient, string categoryName)
        {
            _telemetryClient = telemetryClient;
            _categoryName = categoryName;
            _severityLevels = new Dictionary<LogLevel, SeverityLevel>()
            {
                { LogLevel.Debug, SeverityLevel.Verbose },
                { LogLevel.Information, SeverityLevel.Information },
                { LogLevel.Warning, SeverityLevel.Warning },
                { LogLevel.Error, SeverityLevel.Error },
                { LogLevel.Critical, SeverityLevel.Critical }
            };
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }


        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var message = formatter(state, exception);
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            var severityLevel = _severityLevels[logLevel];

            var telemetry = new TraceTelemetry(message, severityLevel);
            telemetry.Properties.Add("CategoryName", _categoryName);
            _telemetryClient.TrackTrace(telemetry);
        }
    }
}
