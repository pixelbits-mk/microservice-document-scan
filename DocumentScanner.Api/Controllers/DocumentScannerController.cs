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
    public class DocumentScannerController : BaseController
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
                var scanResult = await _scanningService.ScanFile(file);
                if (scanResult.Success)
                {
                    return Ok("File is clean.");
                }
                else
                {
                    return BadRequest($"File is infected. Reason: {scanResult.FailureReason}");
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

        [HttpPost]
        [Route("ScanMultipleFiles")]
        public async Task<IActionResult> ScanMultipleFiles([FromForm] List<IFormFile> files)
        {
            try
            {
                if (files == null || files.Count == 0)
                {
                    return BadRequest("No files provided");
                }

                var scanResult = await _scanningService.ScanMultipleFiles(files);

                if (!scanResult.Success)
                {
                    // If any file is infected, return a BadRequest response
                    return BadRequest($"File is infected. Reason: {scanResult.FailureReason}");
                }

                // If all files are clean, return an Ok response
                return Ok("All files are clean.");
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
                    //byte[] binaryData = ms.ToArray();
                    //if (binaryData == null || binaryData.Length == 0)
                    //{
                    //    return BadRequest("No binary data provided");
                    //}
                    var scanResult = await _scanningService.ScanStream(ms);

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
        [HttpPost]
        [Route("ScanRemoteUrl")]
        public async Task<IActionResult> ScanRemoteUrl([FromQuery] string url)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                {
                    return BadRequest("No URL provided");
                }

                var scanResult = await _scanningService.ScanRemoteUrl(url);

                if (scanResult.Success)
                {
                    return Ok("File is clean.");
                }
                else
                {
                    return BadRequest($"File is infected. Reason: {scanResult.FailureReason}");
                }
            }
            catch (TimeoutException)
            {
                return StatusCode(408, "Scan operation timed out");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during URL scanning");
                throw;
            }
        }   
    }
}
