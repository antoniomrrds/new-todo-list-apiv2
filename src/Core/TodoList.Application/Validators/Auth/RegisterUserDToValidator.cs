using FluentValidation;
using TodoList.Application.DTOs.Auth;
using TodoList.Application.ports.Repositories;

namespace TodoList.Application.Validators.Auth
{
    public class RegisterUserDToValidator : AbstractValidator<RegisterUserDTo>
    {
        public RegisterUserDToValidator(IUserRepository userRepository)
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("O email é obrigatório.")
                .Matches(@"^\S.*$").WithMessage("O email não pode conter apenas espaços em branco. É necessário um valor válido.")
                .EmailAddress().WithMessage("O email informado não é válido.")
                .CustomAsync(async (email, context, cancellationToken) =>
                {
                    // Assuming this is an async method, so we use await
                    var emailExists = await userRepository.DoesEmailExist(email);
                    if (emailExists)
                    {
                        context.AddFailure("O email informado já está em uso.");
                    }
                });

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
}
