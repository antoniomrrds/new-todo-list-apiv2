using FluentValidation;

namespace TodoList.Application.Validators.Shared
{
    public class ConfirmPasswordValidator : AbstractValidator<string>
    {
        public ConfirmPasswordValidator()
        {
            RuleFor(x => x)
                .NotEmpty().WithMessage("A confirmação de senha é obrigatória.")
                .SetValidator(new NoWhiteSpaceValidator());
        }
    }
}
