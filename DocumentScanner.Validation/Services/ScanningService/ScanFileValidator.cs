using DocumentScanner.Validation.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentScanner.Validation.Services.ScanningService
{
    public class ScanFileValidator : ValidatorBase<IFormFile>, Interfaces.IValidator<IFormFile>
    {
        public override ValidatorId ValidatorId
        {
            get
            {
                return ValidatorId.ScanFile;
            }
        }

        public ScanFileValidator()
        {

        }
    }
}
