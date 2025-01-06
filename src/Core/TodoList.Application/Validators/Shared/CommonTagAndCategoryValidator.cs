using FluentValidation;
using TodoList.Application.Ports.Validators;

namespace TodoList.Application.Validators.Shared;

public abstract class CommonTagAndCategoryValidator<T> : AbstractValidator<T> where T : CommonPropertiesTagAndCategory
{
    protected CommonTagAndCategoryValidator() {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O Nome é obrigatório.")
            .MaximumLength(100).WithMessage("O Nome deve ter no máximo 100 caracteres.");
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("A Descrição é obrigatória.")
            .MaximumLength(500).WithMessage("A Descrição deve ter no máximo 500 caracteres.");
        RuleFor(x => x.Active)
            .IsInEnum().WithMessage("O Campo Active deve ser 'Active' (1) ou 'Inactive' (0).");
    }
}

