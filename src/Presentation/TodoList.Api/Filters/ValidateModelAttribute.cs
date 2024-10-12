using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TodoList.Api.Filters
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            foreach (var arg in context.ActionArguments.Values)
            {
                if (arg == null) continue;

                var validator = GetValidator(arg.GetType(), context);
                if (validator == null) continue;

                var validationResult = validator.Validate(new ValidationContext<object>(arg));
                if (validationResult.IsValid) continue;

                var errors = ExtractErrors(validationResult);
                context.Result = new BadRequestObjectResult(new 
                {
                    success = false,
                    statusCode = StatusCodes.Status400BadRequest,
                    message = "A solicitação contém erros de validação.",
                    errors
                });
            }
        }

        private IValidator? GetValidator(Type type, ActionExecutingContext context)
        {
            var validatorType = typeof(IValidator<>).MakeGenericType(type);
            return context.HttpContext.RequestServices.GetService(validatorType) as IValidator;
        }

        private Dictionary<string, List<string>> ExtractErrors(FluentValidation.Results.ValidationResult validationResult)
        {
            return validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToList());
        }
    }
}