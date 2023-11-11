using ClamAV.Net.Client;
using AvScanner.Application.Interfaces;
using AvScanner.Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging; // Include the appropriate logging namespace
using Microsoft.ApplicationInsights;
using System.IO;

namespace AvScanner.Application.Services
{
    public class ScanningService : IScanningService
    {
        private readonly IClamAvClient _clamAvClient;
        private readonly ILogger<ScanningService> _logger; // Add a logger
        private readonly TelemetryClient _telemetryClient;

        public ScanningService(IClamAvClient clamAvClient, ILogger<ScanningService> logger, TelemetryClient telemetryClient)
        {
            _clamAvClient = clamAvClient;
            _logger = logger;
            _telemetryClient = telemetryClient;
        }
        private async Task<ClamAV.Net.Client.Results.ScanResult> PerformScan(Stream stream, int maxRetries = 2)
        {
            int retryCount = 0;

            while (retryCount <= maxRetries)
            {
                var startTime = DateTime.UtcNow;

                try
                {
                    // Your scan logic here
                    return await _clamAvClient.ScanDataAsync(stream);
                }
                catch (Exception ex)
                {
                    // Log the exception (adjust the logging based on your needs)
                    _logger.LogError(ex, $"Error during scan attempt {retryCount + 1}");

                    // Increment the retry count
                    retryCount++;

                    // If this was the last retry, rethrow the exception
                    if (retryCount > maxRetries)
                    {
                        _telemetryClient.GetMetric("ScanErrors").TrackValue(1);
                        throw;
                    }
                }
                finally
                {
                    var endTime = DateTime.UtcNow;
                    var scanDuration = endTime - startTime;

                    // Log ScanTime
                    _telemetryClient.GetMetric("ScanTime").TrackValue(scanDuration.TotalMilliseconds);
                }
            }

            // This point is reached only if all retry attempts fail
            throw new InvalidOperationException("Exceeded maximum number of retry attempts");
        }

        public async Task<ScanResult> ScanData(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return new ScanResult { Success = false, FailureReason = "Invalid data" };
            }

            using (var memoryStream = new MemoryStream(data))
            {
                var scanResult = await PerformScan(memoryStream);

                if (scanResult.Infected)
                {
                    _logger.LogWarning($"File is infected. Virus name: {scanResult.VirusName}");
                    _telemetryClient.GetMetric("ScanFailures").TrackValue(1);
                    return new ScanResult { Success = false, FailureReason = $"File is infected. Virus name: {scanResult.VirusName}" };
                }
                _telemetryClient.GetMetric("ScansCompleted").TrackValue(1);
                return new ScanResult { Success = true };
            }
        }

        public async Task<Models.ScanResult> ScanFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentNullException(nameof(file), "No file data provided");
            }

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);

                    var scanResult = await PerformScan(memoryStream);

                    if (scanResult.Infected)
                    {
                        _logger.LogWarning($"File is infected. Virus name: {scanResult.VirusName}");
                        _telemetryClient.GetMetric("ScanFailures").TrackValue(1);
                        return new ScanResult { Success = false, FailureReason = $"File is infected. Virus name: {scanResult.VirusName}" };
                    }
                    _telemetryClient.GetMetric("ScansCompleted").TrackValue(1);
                    return new ScanResult { Success = true };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error scanning file: {ex.Message}");
                throw; // Rethrow the exception after logging
            }
        }

        public async Task<ScanResult> ScanMultipleFiles(List<IFormFile> files)
        {
            try
            {
                if (files == null || files.Count == 0)
                {
                    throw new ArgumentNullException(nameof(files), "No file data provided");
                }

                var scanTasks = files.Select(file => ScanFile(file));

                var scanResults = await Task.WhenAll(scanTasks);

                // Check if any file is infected
                if (scanResults.Any(result => !result.Success))
                {
                    // If any file is infected, return a failure result
                    _telemetryClient.GetMetric("ScanFailures").TrackValue(1);
                    return new ScanResult { Success = false, FailureReason = "One or more files are infected" };
                }

                _telemetryClient.GetMetric("ScansCompleted").TrackValue(1);
                // If all files are clean, return a success result
                return new ScanResult { Success = true };
            }
            catch (TimeoutException)
            {
                return new ScanResult { Success = false, FailureReason = "Scan operation timed out" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during file scanning");
                throw;
            }
        }

        public async Task<ScanResult> ScanMultipleRemoteUrls(string[] urls)
        {
            try
            {
                if (urls.Length == 0)
                {
                    throw new ArgumentNullException(nameof(urls), "No URLs provided");
                }

                var scanTasks = urls.Select(url => ScanRemoteUrl(url));

                var scanResults = await Task.WhenAll(scanTasks);

                // Check if any file is infected
                if (scanResults.Any(result => !result.Success))
                {
                    _telemetryClient.GetMetric("ScanFailures").TrackValue(1);
                    // If any file is infected, return a failure result
                    return new ScanResult { Success = false, FailureReason = "One or more URLs are infected" };
                }

                _telemetryClient.GetMetric("ScansCompleted").TrackValue(1);
                // If all files are clean, return a success result
                return new ScanResult { Success = true };
            }
            catch (TimeoutException)
            {
                return new ScanResult { Success = false, FailureReason = "Scan operation timed out" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during file scanning");
                throw;
            }
        }

        public async Task<ScanResult> ScanRemoteUrl(string remoteUrl)
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    byte[] fileData = await httpClient.GetByteArrayAsync(remoteUrl);

                    using (var memoryStream = new MemoryStream(fileData))
                    {
                        var scanResult = await PerformScan(memoryStream);

                        if (scanResult.Infected)
                        {
                            _telemetryClient.GetMetric("ScanFailures").TrackValue(1);
                            _logger.LogWarning($"File is infected. Virus name: {scanResult.VirusName}");
                            return new ScanResult { Success = false, FailureReason = $"File is infected. Virus name: {scanResult.VirusName}" };
                        }

                        _telemetryClient.GetMetric("ScansCompleted").TrackValue(1);
                        return new ScanResult { Success = true };
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error scanning remote URL: {ex.Message}");
                    throw; // Rethrow the exception after logging
                }
            }
        }

        public async Task<Models.ScanResult> ScanStream(Stream stream)
        {
            try
            {
                var scanResult = await PerformScan(stream);

                if (scanResult.Infected)
                {
                    _logger.LogWarning($"File is infected. Virus name: {scanResult.VirusName}");
                    _telemetryClient.GetMetric("ScanFailures").TrackValue(1);
                    return new ScanResult { Success = false, FailureReason = $"File is infected. Virus name: {scanResult.VirusName}" };
                }

                _telemetryClient.GetMetric("ScansCompleted").TrackValue(1);
                return new ScanResult { Success = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error scanning stream: {ex.Message}");
                throw; // Rethrow the exception after logging
            }
        }
    }
}
