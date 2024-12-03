using FluentValidation;
using TodoList.Application.DTOs.Todo;
using TodoList.Application.ports.Repositories;

namespace TodoList.Application.Validators.Todo;

public class CreateTodoDToValidator :TodoDToValidatorBase<CreateTodoDTo>
{
    public CreateTodoDToValidator(ITagRepository tagRepository, ICategoryRepository categoryRepository)
    {
        RuleFor(x => x.IdTags)
            .MustAsync(async (tags, _) =>
            {
                if (tags == null || tags.Count == 0) return true; 
                var missingTags = await tagRepository.AreAllEntitiesPresentAsync(tags);
                return !missingTags.Any(); 
            })
            .WithMessage((_, tags) =>
            {
                var missingTags = tagRepository.AreAllEntitiesPresentAsync((tags ?? [])).Result.ToList();
                return missingTags.Count != 0
                    ? $"Uma ou mais tags informadas não estão cadastradas. IDs ausentes: {string.Join(", ", missingTags)}."
                    : string.Empty;
            });

        RuleFor(x => x.IdCategories)
            .MustAsync(async (categories, _) =>
            {
                if (categories == null || categories.Count == 0) return true;
                var missingCategories = await categoryRepository.AreAllEntitiesPresentAsync(categories);
                return !missingCategories.Any();
            })
            .WithMessage((_, categories) =>
            {
                var missingCategories = categoryRepository.AreAllEntitiesPresentAsync((categories ?? [])).Result.ToList();
                return missingCategories.Count != 0
                    ? $"Uma ou mais categorias informadas não estão cadastradas. IDs ausentes: {string.Join(", ", missingCategories)}."
                    : string.Empty;
            });
    }
}