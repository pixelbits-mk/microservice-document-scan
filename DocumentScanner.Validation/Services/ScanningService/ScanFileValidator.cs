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
            RuleFor(file => file)
                .NotNull().WithMessage("File cannot be null")
                .Must(file => file.Length > 0).WithMessage("File cannot be empty");

            RuleFor(file => file.FileName)
                .NotEmpty().WithMessage("File name cannot be empty")
                .MaximumLength(255).WithMessage("File name cannot exceed 255 characters");

            RuleFor(file => file.ContentType)
                .Must(IsValidContentType).WithMessage("Invalid file content type");

            RuleFor(file => file.Length)
                .Must(length => length <= 10 * 1024 * 1024) // 10 MB limit, adjust as needed
                .WithMessage("File size cannot exceed 10 MB");
        }
        private bool IsValidContentType(string contentType)
        {
            // Add logic to check if the content type is valid based on your requirements
            // For example, you might check if it's an allowed MIME type.
            // This is a simple example, and you may need to customize it.
            return ContentTypes.All.Contains(contentType);
        }
    }
}
