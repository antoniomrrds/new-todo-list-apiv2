using FluentValidation;
using TodoList.Application.ports.Repositories;

namespace TodoList.Application.Validators.Shared
{
    public class EmailValidator : AbstractValidator<string>
    {
        public EmailValidator(IUserRepository userRepository)
        {
            RuleFor(x => x)
                .NotEmpty().WithMessage("O email é obrigatório.")
                .SetValidator(new NoWhiteSpaceValidator())
                .EmailAddress().WithMessage("O email informado não é válido.")
                .CustomAsync(async (email, context, cancellationToken) =>
                {
                    var emailExists = await userRepository.DoesEmailExist(email);
                    if (emailExists)
                    {
                        context.AddFailure("O email informado já está em uso.");
                    }
                });
        }
    }
}
