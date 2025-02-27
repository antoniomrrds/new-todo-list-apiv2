using FluentValidation;
using TodoList.Application.DTOs.User;
using TodoList.Application.Validators.Shared;

namespace TodoList.Application.Validators.User;

public class ChangePasswordValidator:AbstractValidator<ChangePasswordDTo>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.Password)
            .SetValidator(new PasswordValidator());
    }
}
