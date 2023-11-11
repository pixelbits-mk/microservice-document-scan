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
            RuleFor(data => data)
                .NotNull().WithMessage("Data cannot be null")
                .NotEmpty().WithMessage("Data cannot be empty");

            RuleFor(data => data.Length)
                .GreaterThan(0).WithMessage("Data length must be greater than 0");


        }

    }
}
