using AvScanner.Validation.Models;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AvScanner.Validation.Services.ScanningService
{
    public class ScanRemoteUrlValidator : ValidatorBase<string>, Interfaces.IValidator<string>
    {
        public override ValidatorId ValidatorId
        {
            get
            {
                return ValidatorId.ScanRemoteUrl;
            }
        }

        public ScanRemoteUrlValidator()
        {
            RuleFor(url => url)
                .NotEmpty().WithMessage("URL cannot be empty")
                .Must(BeValidUrl).WithMessage("Invalid URL format");

        }

        private bool BeValidUrl(string url)
        {
            return Regex.IsMatch(url, RegExConstants.RemoteUrl);
        }
    }
}
