using FluentValidation;
using TodoList.Application.DTOs.Todo;
using TodoList.Application.ports.Repositories;
using TodoList.Domain.Entities;

namespace TodoList.Application.Validators.Todo;

public abstract class TodoDToValidatorBase<T> : AbstractValidator<T> where T : ITodo
{
    protected TodoDToValidatorBase()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("O título é obrigatório.")
            .MaximumLength(100).WithMessage("O título deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("A descrição é obrigatória.")
            .MinimumLength(5).WithMessage("A descrição deve ter no mínimo 5 caracteres.");

        RuleFor(x => (int)x.Active)
            .InclusiveBetween(0, 1).WithMessage("O Campo Active deve ser 0 (inativo) ou 1 (ativo).");
    }
}

public abstract class TodoDToBaseWithTagAndCategoryIds<T> : AbstractValidator<T> where T : TagAndCategoryIdsDto
{
    protected TodoDToBaseWithTagAndCategoryIds(ITagRepository tagRepository, ICategoryRepository categoryRepository)
    {
        
        RuleFor(x => x.IdTags)
            .CustomAsync(async (tags, context, _) => await IsAllPresent(tags, context, tagRepository, "tags"));

        RuleFor(x => x.IdCategories)
            .CustomAsync(async (categories, context, _) => await IsAllPresent(categories, context, categoryRepository, "categorias"));
    }
    
    private static async Task IsAllPresent(List<int>? ids,  ValidationContext<T>  ctx ,IEntityExistenceCheckerRepository entityExistenceCheckerRepository,string entityName )
    {
        if (ids == null || ids.Count == 0) return ;
        var missingValues = (await entityExistenceCheckerRepository.AreAllEntitiesPresentAsync(ids)).ToList();
        if (missingValues.Count > 0)
        {
            ctx.AddFailure(ShowMessageError(missingValues, entityName));
        }
    }

    private static string ShowMessageError(List<int> missingValues, string entityName)
            => missingValues.Count != 0
                ? $"Uma ou mais {entityName} informadas não estão cadastradas. IDs ausentes: {string.Join(", ", missingValues)}."
                : string.Empty;
}