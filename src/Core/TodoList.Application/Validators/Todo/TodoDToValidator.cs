using FluentValidation;
using TodoList.Application.DTOs.Todo;
using TodoList.Application.ports.Repositories;

namespace TodoList.Application.Validators.Todo;

public class TodoDToValidator : AbstractValidator<CreateTodoDTo>
{
    private readonly ITagRepository _tagRepository;
    
    public   TodoDToValidator(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
        
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("O título é obrigatório.")
            .MaximumLength(100).WithMessage("O título deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("A descrição é obrigatória.")
            .MinimumLength(5).WithMessage("A descrição deve ter no mínimo 5 caracteres.")
            .MaximumLength(500).WithMessage("A descrição deve ter no máximo 500 caracteres.");

        RuleFor(x => x.IsCompleted)
            .NotEmpty().WithMessage("O status de conclusão é obrigatório.");
            
        RuleFor(x => x.Status)
            .InclusiveBetween(0, 1).WithMessage("O status deve ser 0 (inativo) ou 1 (ativo).");

        RuleFor(x => x.IdTag)
            .GreaterThan(0).When(x => x.IdTag > 0).WithMessage("O ID da tag deve ser um valor positivo, se informado.");

        RuleFor(x => x.IdCategory)
            .GreaterThan(0).When(x => x.IdCategory > 0)
            .WithMessage("O ID da categoria deve ser um valor positivo, se informado.");
        
        RuleFor(x => x.IdTag)
            .GreaterThan(0).When(x => x.IdTag > 0)
            .WithMessage("O ID da tag deve ser um valor positivo, se informado.")
            .MustAsync(TagExists).WithMessage("A tag informada não está cadastrada. Verifique o ID da tag.");
        RuleFor(x => x.IdCategory)
            .GreaterThan(0).When(x => x.IdCategory > 0).WithMessage("O ID da categoria deve ser um valor positivo, se informado.");

    }
    
    private async Task<bool> TagExists(int? idTag, CancellationToken cancellationToken)
    {
        if (idTag is null)
        {
            return true;
        }

        var tag = await _tagRepository.GetByIdAsync(idTag.Value);
        return tag is not null;
    }
}
