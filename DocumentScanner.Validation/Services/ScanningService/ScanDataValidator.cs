using DocumentScanner.Validation.Models;
using FluentValidation;
using FluentValidation.Results;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentScanner.Validation.Services.ScanningService
{
    public class ScanDataValidator : ValidatorBase<byte[]>, Interfaces.IValidator<byte[]>
    {
        public override ValidatorId ValidatorId
        {
            get
            {
                return ValidatorId.ScanData;
            }
        }
        public ScanDataValidator()
        {
            RuleFor(c => c)
                .Must(c => c != null && c.Length > 0)
                .WithMessage("data cannot be empty");
           
        }

    }
}
