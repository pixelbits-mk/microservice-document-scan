using AvScanner.Validation.Models;
using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvScanner.Validation.Services
{
    public abstract class ValidatorBase<T> : AbstractValidator<T>, Interfaces.IValidator
    {
        public abstract ValidatorId ValidatorId { get; }

        public ValidationResult Validate(object model)
        {
            return base.Validate((T)model);
        }

    }
}
