
using FluentValidation;
using TodoList.Application.DTOs.Auth;
using TodoList.Application.ports.Repositories;
using TodoList.Application.Validators.Shared;


namespace TodoList.Application.Validators.Auth;

public class SignInDToValidator: AbstractValidator<SignInDTo>
{
    public SignInDToValidator(IUserRepository userRepository)
    {
        RuleFor(x => x.Email)
            .SetValidator(new EmailValidator())
            .CustomAsync(async (email, context, _) =>
        {
            var emailExists = await userRepository.DoesEmailExist(email);
            if (!emailExists)
            {
                context.AddFailure("O email informado não está cadastrado.");
            }
        });

        RuleFor(x => x.Password)
            .SetValidator(new PasswordValidator());
    }
}
