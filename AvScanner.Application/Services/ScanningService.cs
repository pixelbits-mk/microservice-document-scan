using ClamAV.Net.Client;
using AvScanner.Application.Interfaces;
using AvScanner.Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging; // Include the appropriate logging namespace


namespace AvScanner.Application.Services
{
    public class ScanningService : IScanningService
    {
        private readonly IClamAvClient _clamAvClient;
        private readonly ILogger<ScanningService> _logger; // Add a logger

        public ScanningService(IClamAvClient clamAvClient, ILogger<ScanningService> logger)
        {
            _clamAvClient = clamAvClient;
            _logger = logger;
        }

        public async Task<Models.ScanResult> ScanData(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return new Models.ScanResult { Success = false, FailureReason = "Invalid data" };
            }

            using (var memoryStream = new MemoryStream(data))
            {
                var scanResult = await _clamAvClient.ScanDataAsync(memoryStream);

                if (scanResult.Infected)
                {
                    _logger.LogWarning($"File is infected. Virus name: {scanResult.VirusName}");
                    return new Models.ScanResult { Success = false, FailureReason = $"File is infected. Virus name: {scanResult.VirusName}" };
                }

                return new Models.ScanResult { Success = true };
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

                    var scanResult = await _clamAvClient.ScanDataAsync(memoryStream);

                    if (scanResult.Infected)
                    {
                        _logger.LogWarning($"File is infected. Virus name: {scanResult.VirusName}");
                        return new Models.ScanResult { Success = false, FailureReason = $"File is infected. Virus name: {scanResult.VirusName}" };
                    }
                    return new Models.ScanResult { Success = true };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error scanning file: {ex.Message}");
                throw; // Rethrow the exception after logging
            }
        }

        public async Task<Models.ScanResult> ScanMultipleFiles(List<IFormFile> files)
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
                    return new Models.ScanResult { Success = false, FailureReason = "One or more files are infected" };
                }

                // If all files are clean, return a success result
                return new Models.ScanResult { Success = true };
            }
            catch (TimeoutException)
            {
                return new Models.ScanResult { Success = false, FailureReason = "Scan operation timed out" };
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
                    // If any file is infected, return a failure result
                    return new Models.ScanResult { Success = false, FailureReason = "One or more URLs are infected" };
                }

                // If all files are clean, return a success result
                return new Models.ScanResult { Success = true };
            }
            catch (TimeoutException)
            {
                return new Models.ScanResult { Success = false, FailureReason = "Scan operation timed out" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during file scanning");
                throw;
            }
        }

        public async Task<Models.ScanResult> ScanRemoteUrl(string remoteUrl)
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    byte[] fileData = await httpClient.GetByteArrayAsync(remoteUrl);

                    using (var memoryStream = new MemoryStream(fileData))
                    {
                        var scanResult = await _clamAvClient.ScanDataAsync(memoryStream);

                        if (scanResult.Infected)
                        {
                            _logger.LogWarning($"File is infected. Virus name: {scanResult.VirusName}");
                            return new Models.ScanResult { Success = false, FailureReason = $"File is infected. Virus name: {scanResult.VirusName}" };
                        }

                        return new Models.ScanResult { Success = true };
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
                var scanResult = await _clamAvClient.ScanDataAsync(stream);

                if (scanResult.Infected)
                {
                    _logger.LogWarning($"File is infected. Virus name: {scanResult.VirusName}");
                    return new Models.ScanResult { Success = false, FailureReason = $"File is infected. Virus name: {scanResult.VirusName}" };
                }

                return new Models.ScanResult { Success = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error scanning stream: {ex.Message}");
                throw; // Rethrow the exception after logging
            }
        }
    }
}
