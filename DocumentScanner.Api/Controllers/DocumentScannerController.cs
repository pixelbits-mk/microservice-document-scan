using DocumentScanner.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DocumentScanner.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DocumentScannerController : ControllerBase
    {
        private readonly IScanningService _scanningService;
        private readonly ILogger<DocumentScannerController> _logger;

        public DocumentScannerController(ILogger<DocumentScannerController> logger, IScanningService scanningService)
        {
            _logger = logger;
            _scanningService = scanningService;
        }

        [HttpPost]
        [Route("ScanData")]
        public async Task<IActionResult> ScanData(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file data provided");
                }

                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    var fileBytes = memoryStream.ToArray();

                    var scanResult = await _scanningService.Scan(fileBytes);

                    if (scanResult.Success)
                    {
                        return Ok("File is clean.");
                    }
                    else
                    {
                        return BadRequest($"File is infected. Reason: {scanResult.FailureReason}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during file scanning");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
