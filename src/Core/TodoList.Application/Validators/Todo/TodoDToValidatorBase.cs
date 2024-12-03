using FluentValidation;
using TodoList.Domain.Entities;

namespace TodoList.Application.Validators.Todo;

public abstract class TodoDToValidatorBase<T> : AbstractValidator<T> where T : ITodo
{
    protected TodoDToValidatorBase()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("O título é obrigatório.")
            .MaximumLength(100).WithMessage("O título deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("A descrição é obrigatória.")
            .MinimumLength(5).WithMessage("A descrição deve ter no mínimo 5 caracteres.")
            .MaximumLength(500).WithMessage("A descrição deve ter no máximo 500 caracteres.");

        RuleFor(x => x.IsCompleted)
            .NotNull().WithMessage("O status de conclusão é obrigatório.");

        RuleFor(x => x.Active)
            .InclusiveBetween(0, 1).WithMessage("O Campo Ativo deve ser 0 (inativo) ou 1 (ativo).");
    }
}

