using AvScanner.Validation.Models;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvScanner.Validation.Interfaces
{
    public interface IValidator
    {
        ValidatorId ValidatorId { get; }

        ValidationResult Validate(object model);
    }

    public interface IValidator<T> : IValidator
    {
        ValidationResult Validate(T model);
    }
}
