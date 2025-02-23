using FluentValidation;
using TodoList.Application.DTOs.Todo;
using TodoList.Application.ports.Repositories;
using TodoList.Domain.Entities;

namespace TodoList.Application.Validators.Todo;

public abstract class TodoDToValidatorBase<T> : AbstractValidator<T> where T : ITodo
{
    protected TodoDToValidatorBase()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MinimumLength(5).WithMessage("O nome deve ter no mínimo 5 caracteres.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("A descrição é obrigatória.")
            .MinimumLength(5).WithMessage("A descrição deve ter no mínimo 5 caracteres.");

        RuleFor(x => x.Active)
            .IsInEnum().WithMessage("O Campo Active deve ser 'Active' (1) ou 'Inactive' (0).");
        RuleFor(x => x.IsCompleted)
            .IsInEnum().WithMessage("O campo IsCompleted deve ser 'Incomplete' (0) ou 'Completed' (1).");

    }
}

public abstract class TodoDToBaseWithTagAndCategoryIds<T> : TodoDToValidatorBase<T> where T : TagAndCategoryIdsDto,ITodo
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
