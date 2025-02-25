using FluentValidation;
using TodoList.Application.DTOs.User;
using TodoList.Application.Validators.Shared;

namespace TodoList.Application.Validators.User;

public class ChangePassword:AbstractValidator<ChangePasswordDTo>
{
    public ChangePassword()
    {
        RuleFor(x => x.Password)
            .SetValidator(new PasswordValidator());
    }
}
