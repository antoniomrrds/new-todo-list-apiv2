using FluentValidation;
using TodoList.Application.DTOs.User;
using TodoList.Application.Validators.Shared;

namespace TodoList.Application.Validators.User;

public class UserNamevalidator:AbstractValidator<UserNameRequestDTo>
{
    public UserNamevalidator()
    {
        RuleFor(x => x.Name)
            .SetValidator(new NameValidator());
    }

}
