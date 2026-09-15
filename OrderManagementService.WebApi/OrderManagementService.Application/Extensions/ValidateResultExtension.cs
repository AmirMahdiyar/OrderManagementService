using FluentValidation.Results;
using OrderManagementService.Application.Exceptions;

namespace OrderManagementService.Application.Extensions
{
    public static class ValidateResultExtension
    {
        public static void ThrowIfNeeded(this ValidationResult validationResult)
        {
            var errors = validationResult.Errors;

            if (errors.Any())
            {
                throw new InputValidationFailedApplicationException(
                    string.Join(Environment.NewLine, errors.Select(e => e.ErrorMessage)));
            }
        }
    }
}
