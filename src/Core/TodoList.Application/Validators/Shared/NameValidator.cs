using FluentValidation;

namespace TodoList.Application.Validators.Shared
{
    public class NameValidator : AbstractValidator<string>
    {
        public NameValidator()
        {
            RuleFor(x => x)
                .NotEmpty().WithMessage("O nome é obrigatório.")
                .MinimumLength(3).WithMessage("O nome deve ter no mínimo 3 caracteres.")
                .MaximumLength(100).WithMessage("O nome deve ter no máximo 100 caracteres.")
                .SetValidator(new NoWhiteSpaceValidator());

        }
    }
}
