using FluentValidation;
using TodoList.Application.DTOs.Tag;

namespace TodoList.Application.Validators.Tag;

public class TagDtoValidator : AbstractValidator<TagCreateDTo>
{
    public TagDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O Nome é obrigatório.")
            .MaximumLength(100).WithMessage("O Nome deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("A descrição é obrigatória.")
            .MaximumLength(500).WithMessage("A descrição deve ter no máximo 500 caracteres.");
        
           RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("O Slug é obrigatório.")
            .MaximumLength(100).WithMessage("O slug deve ter no máximo 100 caracteres.");
           
    }
}

