using FluentValidation;
using TodoList.Application.DTOs.Category;

namespace TodoList.Application.Validators.Category;

public class categoryValidator: AbstractValidator<CategoryDTo>
{
    public categoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O Nome é obrigatório.")
            .MaximumLength(100).WithMessage("O Nome deve ter no máximo 100 caracteres.");
        
    }
}