using FluentValidation;

namespace TodoList.Application.Validators.Shared
{
    public class PasswordValidator : AbstractValidator<string>
    {
        public PasswordValidator()
        {
            RuleFor(x => x)
                .NotEmpty().WithMessage("A senha é obrigatória.")
                .MinimumLength(6).WithMessage("A senha deve ter no mínimo 6 caracteres.")
                .SetValidator(new NoWhiteSpaceValidator());
        }
    }
}
