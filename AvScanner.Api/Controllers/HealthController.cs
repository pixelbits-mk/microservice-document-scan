using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AvScanner.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> CheckHealth()
        {
            // Perform health checks here
            bool isClamAVHealthy = await IsClamAVHealthyAsync();

            if (isClamAVHealthy)
            {
                return Ok("ClamAV is healthy");
            }
            else
            {
                return StatusCode(500, "ClamAV is not healthy");
            }
        }

        private async Task<bool> IsClamAVHealthyAsync()
        {
            try
            {
                // Use the clamdscan command to check the health of ClamAV
                using (Process process = new Process())
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = "clamdscan appsettings.json",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    process.StartInfo = startInfo;
                    process.Start();

                    // Read the output and error streams asynchronously
                    string output = await process.StandardOutput.ReadToEndAsync();
                    string error = await process.StandardError.ReadToEndAsync();

                    // Asynchronously wait for the process to exit
                    await process.WaitForExitAsync();

                    // Check if ClamAV is healthy based on the output
                    return process.ExitCode == 0 && !output.ToLower().Contains("error") && string.IsNullOrEmpty(error);
                }
            }
            catch (Exception ex)
            {
                // Log any exceptions or errors
                // You may want to log the details of the exception for troubleshooting
                Console.WriteLine($"Error checking ClamAV health: {ex.Message}");
                return false;
            }
        }
    }

    public static class ProcessExtensions
    {
        public static Task<bool> WaitForExitAsync(this Process process)
        {
            var tcs = new TaskCompletionSource<bool>();

            process.EnableRaisingEvents = true;
            process.Exited += (sender, args) => tcs.TrySetResult(true);

            return tcs.Task;
        }
    }
}
