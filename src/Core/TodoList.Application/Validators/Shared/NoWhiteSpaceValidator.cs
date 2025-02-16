using FluentValidation;

namespace TodoList.Application.Validators.Shared
{
    public class NoWhiteSpaceValidator : AbstractValidator<string>
    {
        public NoWhiteSpaceValidator()
        {
            RuleFor(x => x)
                .Matches(@"^\S.*$").WithMessage("O valor do campo não pode conter apenas espaços em branco. É necessário um valor válido.");
        }
    }
}
