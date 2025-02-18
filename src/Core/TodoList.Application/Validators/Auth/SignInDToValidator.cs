using FluentValidation;
using TodoList.Application.DTOs.Auth;
using TodoList.Application.Validators.Shared;

namespace TodoList.Application.Validators.Auth;

public class SignInDToValidator: AbstractValidator<SignInDTo>
{
    public SignInDToValidator()
    {
        RuleFor(x => x.Email)
            .SetValidator(new EmailValidator());

        RuleFor(x => x.Password)
            .SetValidator(new PasswordValidator());
    }
}
