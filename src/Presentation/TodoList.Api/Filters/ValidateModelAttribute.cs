using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;
using TodoList.Application.Factories; // Adicionando o namespace da nova classe
using TodoList.Domain.Constants;

namespace TodoList.Api.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        private readonly ILogger<ValidateModelAttribute> _logger;

        public ValidateModelAttribute(ILogger<ValidateModelAttribute> logger)
        {
            _logger = logger;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var modelStateErrors = GetModelStateErrors(context);

            if (modelStateErrors.Count != 0)
            {
                // Usando a classe BadRequestResponseFactory para criar a resposta
                context.Result = BadRequestResponseFactory.CreateBadRequestResponse("Os dados enviados não estão corretos. Verifique o modelo e tente novamente.", modelStateErrors);
                return;
            }

            ValidateActionArguments(context).GetAwaiter().GetResult();
        }

        private static Dictionary<string, List<string>> GetModelStateErrors(ActionExecutingContext context)
        {
            var errors = new Dictionary<string, List<string>>();

            if (context.ModelState.IsValid) return errors;

            foreach (var key in context.ModelState.Keys)
            {
                var stateValue = context.ModelState[key];
                var conversionErrors = stateValue?.Errors
                    .Where(e => e.ErrorMessage.Contains("could not be converted"))
                    .Select(_ => $"O valor do campo '{key.Replace("$.", "")}' está errado. Verifique o modelo e tente novamente.")
                    .ToList();

                if (conversionErrors != null && conversionErrors.Count != DefaultValues.IdNullValue)
                {
                    errors[key.Replace("$.", "")] = conversionErrors;
                }
            }

            return errors;
        }

        private async Task ValidateActionArguments(ActionExecutingContext context)
        {
            foreach (var arg in context.ActionArguments.Values)
            {
                if (arg == null) continue;

                var validator = GetValidator(arg.GetType(), context);
                if (validator == null)
                {
                    _logger.LogWarning("Nenhum validador encontrado para o tipo {Name}", arg.GetType().Name);
                    continue;
                }

                var validationResult = await validator.ValidateAsync(new ValidationContext<object>(arg));
                if (validationResult.IsValid) continue;

                var errors = ExtractErrors(validationResult);
                context.Result = BadRequestResponseFactory.CreateBadRequestResponse("A solicitação contém erros de validação.", errors);
                return;
            }
        }

        private static IValidator? GetValidator(Type type, ActionExecutingContext context)
        {
            var validatorType = typeof(IValidator<>).MakeGenericType(type);
            return context.HttpContext.RequestServices.GetService(validatorType) as IValidator;
        }

        private static Dictionary<string, List<string>> ExtractErrors(FluentValidation.Results.ValidationResult validationResult)
        {
            return validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToList());
        }
    }
}
