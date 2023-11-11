using ClamAV.Net.Client.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentScanner.Application.Interfaces
{
    public interface IScanningService
    {
        Task<Models.ScanResult> Scan(byte[] data);
    }
}
