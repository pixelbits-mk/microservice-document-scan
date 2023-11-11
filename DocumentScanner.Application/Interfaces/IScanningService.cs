using ClamAV.Net.Client.Results;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentScanner.Application.Interfaces
{
    public interface IScanningService
    {
        Task<Models.ScanResult> ScanData(byte[] data);
        Task<Models.ScanResult> ScanFile(IFormFile file);
        Task<Models.ScanResult> ScanMultipleFiles(List<IFormFile> files);
        Task<Models.ScanResult> ScanRemoteUrl(string url);

        Task<Models.ScanResult> ScanMultipleRemoteUrls(string [] url);

        Task<Models.ScanResult> ScanStream(Stream stream);

    }
}
