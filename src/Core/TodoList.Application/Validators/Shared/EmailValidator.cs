using FluentValidation;
using TodoList.Domain.extensions;

namespace TodoList.Application.Validators.Shared
{
    public class EmailValidator : AbstractValidator<string>
    {
        public EmailValidator()
        {
            RuleFor(x => x)
                .NotEmpty().WithMessage("O email é obrigatório.")
                .SetValidator(new NoWhiteSpaceValidator())
                .Must(e => e.IsEmail()).WithMessage("O email informado é inválido.");


        }
    }
}
