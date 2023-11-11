using AvScanner.Validation.Interfaces;
using AvScanner.Validation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvScanner.Validation.Services
{
    public class ValidatorFactory : IValidatorFactory
    {
        private readonly IList<IValidator> _validators;
        public ValidatorFactory(IList<IValidator> validators)
        {
            _validators = validators;
        }
        public IValidator<T> Create<T>(ValidatorId validatorId)
        {
            var validator = _validators.FirstOrDefault(t => t.ValidatorId == validatorId);
            if (validator == null)
            {
                throw new ApplicationException($"Validator {validatorId} not found");
            }

            if (validator is IValidator<T> typedValidator)
            {
                return typedValidator;
            }

            throw new ApplicationException($"Validator {validatorId} is not of type IValidator<{typeof(T).FullName}>");

        }
    }
}
