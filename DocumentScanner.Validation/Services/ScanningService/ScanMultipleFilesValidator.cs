using DocumentScanner.Validation.Models;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentScanner.Validation.Services.ScanningService
{
    public class ScanMultipleFilesValidator : ValidatorBase<List<IFormFile>>, Interfaces.IValidator<List<IFormFile>>
    {
        public override ValidatorId ValidatorId
        {
            get
            {
                return ValidatorId.ScanMultipleFiles;
            }
        }

        public ScanMultipleFilesValidator()
        {
            RuleFor(files => files)
                .NotNull().WithMessage("Files list cannot be null")
                .Must(files => files.Any()).WithMessage("Files list cannot be empty");

            RuleForEach(files => files)
                .SetValidator(new ScanFileValidator()); // You can create a nested validator for individual files

        }
    }
}
