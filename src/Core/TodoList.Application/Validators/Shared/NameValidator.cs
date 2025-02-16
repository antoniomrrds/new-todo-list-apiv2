using FluentValidation;

namespace TodoList.Application.Validators.Shared
{
    public class NameValidator : AbstractValidator<string>
    {
        public NameValidator()
        {
            RuleFor(x => x)
                .NotEmpty().WithMessage("O nome é obrigatório.")
                .MinimumLength(5).WithMessage("O nome deve ter no mínimo 5 caracteres.")
                .SetValidator(new NoWhiteSpaceValidator());
        }
    }
}
