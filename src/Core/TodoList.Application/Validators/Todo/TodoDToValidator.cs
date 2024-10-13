using FluentValidation;
using TodoList.Application.DTOs.Todo;

namespace TodoList.Application.Validators.Todo;

public class TodoDToValidator : AbstractValidator<CreateTodoDTo>
{
    public TodoDToValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("O título é obrigatório.")
            .MaximumLength(100).WithMessage("O título deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("A descrição é obrigatória.")
            .MinimumLength(5).WithMessage("A descrição deve ter no mínimo 5 caracteres.")
            .MaximumLength(500).WithMessage("A descrição deve ter no máximo 500 caracteres.");

        RuleFor(x => x.IsCompleted)
            .NotEmpty().WithMessage("O status de conclusão é obrigatório.")
            .Must(value => IsValidBoolean(value))
            .WithMessage("O status de conclusão deve ser 'true' ou 'false'.");


        RuleFor(x => x.Status)
            .InclusiveBetween(0, 1).WithMessage("O status deve ser 0 (inativo) ou 1 (ativo).");

        RuleFor(x => x.IdTag)
            .GreaterThan(0).When(x => x.IdTag > 0).WithMessage("O ID da tag deve ser um valor positivo, se informado.");

        RuleFor(x => x.IdCategory)
            .GreaterThan(0).When(x => x.IdCategory > 0)
            .WithMessage("O ID da categoria deve ser um valor positivo, se informado.");
    }

    private bool IsValidBoolean(object? value)
    {
        if (value == null) return false;

        if (value is bool)
            return true;

        if (value is string strValue)
            return bool.TryParse(strValue, out _);

        return false;
    }
}