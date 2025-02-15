using FluentValidation;
using TodoList.Application.DTOs.Auth;

namespace TodoList.Application.Validators.Auth;

public class RegisterUserDToValidator: AbstractValidator<RegisterUser>
{
    public RegisterUserDToValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O email é obrigatório.")
            .Matches(@"^\S.*$")
            .WithMessage("O email não pode conter apenas espaços em branco. É necessário um valor válido.")
            .EmailAddress().WithMessage("O email informado não é válido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("A senha é obrigatória.")
            .MinimumLength(6).WithMessage("A senha deve ter no mínimo 6 caracteres.")
            .Matches(@"^\S.*$").WithMessage("A senha não pode conter apenas espaços em branco. É necessário um valor válido.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MinimumLength(5).WithMessage("O nome deve ter no mínimo 5 caracteres.")
            .Matches(@"^\S.*$").WithMessage("O nome não pode conter apenas espaços em branco. É necessário um valor válido.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("A confirmação de senha é obrigatória.")
            .Equal(x => x.Password).WithMessage("As senhas não conferem.")
            .Matches(@"^\S.*$").WithMessage("A confirmação de senha não pode conter apenas espaços em branco. É necessário um valor válido.");
    }
}
