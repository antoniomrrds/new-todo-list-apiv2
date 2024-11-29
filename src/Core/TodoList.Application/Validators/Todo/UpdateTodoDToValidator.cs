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
                var missingCategories =
                    categoryRepository.AreAllEntitiesPresentAsync((categories ?? [])).Result.ToList();
                return missingCategories.Count != 0
                    ? $"Uma ou mais categorias informadas não estão cadastradas. IDs ausentes: {string.Join(", ", missingCategories)}."
                    : string.Empty;
            });
    }
}