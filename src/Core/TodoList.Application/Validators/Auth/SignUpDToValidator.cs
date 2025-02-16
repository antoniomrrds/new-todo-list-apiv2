using FluentValidation;
using TodoList.Application.DTOs.Auth;
using TodoList.Application.ports.Repositories;
using TodoList.Application.Validators.Shared;

namespace TodoList.Application.Validators.Auth
{
    public class SignUpDToValidator : AbstractValidator<SignUpDTo>
    {
        public SignUpDToValidator(IUserRepository userRepository)
        {
            // Validando o Email
            RuleFor(x => x.Email)
                .SetValidator(new EmailValidator(userRepository));

            RuleFor(x => x.Name)
                .SetValidator(new NameValidator());

            RuleFor(x => x.Password)
                .SetValidator(new PasswordValidator());

            // Validando a Confirmação de Senha
            RuleFor(x => x.ConfirmPassword)
                .SetValidator(new ConfirmPasswordValidator())
                .Equal(x => x.Password).WithMessage("As senhas não conferem.");

        }
    }
}
