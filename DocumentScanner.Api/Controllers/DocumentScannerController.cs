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
        [Route("ScanFile")]
        public async Task<IActionResult> ScanFile(IFormFile file)
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
            catch (TimeoutException)
            {
                return StatusCode(408, "Scan operation timed out");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during file scanning");
                throw;
            }
        }

        //[HttpPost]
        //[Route("ScanData")]
        //public async Task<IActionResult> ScanData([FromBody] byte[] binaryData)
        //{
        //    try
        //    {

        //        if (binaryData == null || binaryData.Length == 0)
        //        {
        //            return BadRequest("No binary data provided");
        //        }

        //        var scanResult = await _scanningService.Scan(binaryData);

        //        if (scanResult.Success)
        //        {
        //            return Ok("Data is clean.");
        //        }
        //        else
        //        {
        //            return BadRequest($"Data is infected. Reason: {scanResult.FailureReason}");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error during binary data scanning");
        //        throw;
        //    }
        //}
        [HttpPost]
        [Route("ScanData")]
        public async Task<IActionResult> ScanData()
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    await Request.Body.CopyToAsync(ms);
                    byte[] binaryData = ms.ToArray();
                    if (binaryData == null || binaryData.Length == 0)
                    {
                        return BadRequest("No binary data provided");
                    }
                    var scanResult = await _scanningService.Scan(binaryData);

                    if (scanResult.Success)
                    {
                        return Ok("Data is clean.");
                    }
                    else
                    {
                        return BadRequest($"Data is infected. Reason: {scanResult.FailureReason}");
                    }

                }

            }
            catch (TimeoutException)
            {
                return StatusCode(408, "Scan operation timed out");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during binary data scanning");
                throw;
            }
        }
    }
}
