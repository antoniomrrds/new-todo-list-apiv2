using FluentValidation;
using TodoList.Application.DTOs.Todo;
using TodoList.Application.ports.Repositories;

namespace TodoList.Application.Validators.Todo;


public class UpdateDToValidator : AbstractValidator<UpdateTodoDTo>
{
    public UpdateDToValidator(ITagRepository tagRepository, ICategoryRepository categoryRepository)
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("O título é obrigatório.")
            .MaximumLength(100).WithMessage("O título deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("A descrição é obrigatória.")
            .MinimumLength(5).WithMessage("A descrição deve ter no mínimo 5 caracteres.")
            .MaximumLength(500).WithMessage("A descrição deve ter no máximo 500 caracteres.");

        RuleFor(x => x.IsCompleted)
            .NotNull().WithMessage("O status de conclusão é obrigatório.");

        RuleFor(x => x.Active)
            .InclusiveBetween(0, 1).WithMessage("O Campo Ativo deve ser 0 (inativo) ou 1 (ativo).");

        RuleFor(x => x.IdTags)
            .MustAsync(async (tags, _) => tags == null || tags.Count == 0 ||  await tagRepository.AreAllEntitiesPresentAsync(tags ))
            .WithMessage("Uma ou mais tags informadas não estão cadastradas. Verifique os IDs das tags.");

        RuleFor(x => x.IdCategories)
            .MustAsync(async (categories, _) => categories == null || categories.Count == 0 || await categoryRepository.AreAllEntitiesPresentAsync(categories))
            .WithMessage("Uma ou mais categorias informadas não estão cadastradas. Verifique os IDs das categorias.");
    }
}
