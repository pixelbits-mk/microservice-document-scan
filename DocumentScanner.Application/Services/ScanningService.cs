using ClamAV.Net.Client;
using ClamAV.Net.Client.Results;
using DocumentScanner.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DocumentScanner.Application.Services
{
    public class ScanningService : IScanningService
    {
        private readonly IClamAvClient _clamAvClient;

        public ScanningService(IClamAvClient clamAvClient)
        {
            _clamAvClient = clamAvClient;
        }
        public async Task<Models.ScanResult> Scan(byte[] data)
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
                    return new Models.ScanResult { Success = false, FailureReason = $"File is infected. Virus name: {scanResult.VirusName}" };
                }

                return new Models.ScanResult { Success = true };
            }
        }

    }
}
