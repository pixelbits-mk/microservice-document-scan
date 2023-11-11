using DocumentScanner.Validation.Models;
using FluentValidation.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentScanner.Validation.Services.ScanningService
{
    public class ScanMultipleRemoteUrlsValidator : ValidatorBase<string[]>, Interfaces.IValidator<string[]>
    {
        public override ValidatorId ValidatorId
        {
            get
            {
                return ValidatorId.ScanMultipleRemoteUrls;
            }
        }

        public ScanMultipleRemoteUrlsValidator()
        {

            RuleFor(urls => urls)
                .NotNull().WithMessage("URLs array cannot be null")
                .Must(urls => urls.Any()).WithMessage("URLs array cannot be empty");
            RuleForEach(urls => urls)
                .SetValidator(new ScanRemoteUrlValidator()); // Reuse ScanRemoteUrlValidator for each URL

        }


    }
}
