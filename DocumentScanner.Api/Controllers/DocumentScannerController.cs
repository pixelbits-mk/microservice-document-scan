using DocumentScanner.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> ScanData([FromBody] byte[] fileBytes)
        {
            try
            {
                if (fileBytes == null || fileBytes.Length == 0)
                {
                    return BadRequest("No file data provided");
                }

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during file scanning");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}