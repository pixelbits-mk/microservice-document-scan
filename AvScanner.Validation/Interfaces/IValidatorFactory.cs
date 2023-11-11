using AvScanner.Validation.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvScanner.Validation.Interfaces
{
    public interface IValidatorFactory
    {
        IValidator<T> Create<T>(ValidatorId validatorId);
    }
}
