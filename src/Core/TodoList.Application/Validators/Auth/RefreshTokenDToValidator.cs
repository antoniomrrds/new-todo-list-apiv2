
using FluentValidation;
using TodoList.Application.DTOs.Auth;
using TodoList.Application.Validators.Shared;

namespace TodoList.Application.Validators.Auth;

public class RefreshTokenDToValidator: AbstractValidator<RefreshTokenDTo>
{
    public RefreshTokenDToValidator()
    {
        RuleFor(x =>x.RefreshToken )
            .SetValidator(new NoWhiteSpaceValidator())
            .NotEmpty().WithMessage("O token de atualização é obrigatório.");
    }
}
