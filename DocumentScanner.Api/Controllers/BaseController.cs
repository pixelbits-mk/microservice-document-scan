using DocumentScanner.Validation.Models;
using DocumentScanner.Validation.Interfaces;
using FluentValidation.Results;

namespace DocumentScanner.Api.Controllers
{
    public class BaseController : Microsoft.AspNetCore.Mvc.ControllerBase
    {

        private IValidatorFactory ValidatorFactory
        {
            get
            {
                var service = HttpContext.RequestServices.GetService<Validation.Interfaces.IValidatorFactory>();
                return service == null ? throw new ApplicationException("RequestServices is null") : service;
            }
        }

        public ValidationResult Validate<T>(ValidatorId validatorId, T model)
        {
            var validator = ValidatorFactory.Create<T>(validatorId);
            return validator.Validate(model);
        }

    }
}
