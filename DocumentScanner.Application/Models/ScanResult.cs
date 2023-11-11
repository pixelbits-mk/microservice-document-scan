using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentScanner.Application.Models
{
    public class ScanResult
    {
        public bool Success { get; set; }
        public string? FailureReason { get; set; }

        public ScanResult()
        {
            Success = false;
            FailureReason = null;
        }

    }
}
