using FluentValidation;
using TodoList.Application.DTOs.Todo;
using TodoList.Application.ports.Repositories;

namespace TodoList.Application.Validators.Todo
{
    public class TodoDToValidator : AbstractValidator<CreateTodoDTo>
    {
        public TodoDToValidator(ITagRepository tagRepository)
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

            RuleFor(x => x.Status)
                .InclusiveBetween(0, 1).WithMessage("O status deve ser 0 (inativo) ou 1 (ativo).");
           
            RuleFor(x => x.IdTag)
                .Cascade(CascadeMode.Stop)
                .Must(id => id is > 0)
                .WithMessage("O ID da tag deve ser um valor positivo e, se informado, deve ser válido.")
                .MustAsync(async (id, _) => id.HasValue && await EntityExistsAsync(tagRepository, id.Value))
                .WithMessage("A tag informada não está cadastrada. Verifique o ID da tag.");

            /*
            RuleFor(x => x.IdCategory)
             .Cascade(CascadeMode.Stop)
                .Must(id => id is > 0)
                .WithMessage("O ID da categoria deve ser um valor positivo e, se informado, deve ser válido.")
                .MustAsync(async (id, _) =>  id.HasValue && await EntityExistsAsync(_categoryRepository, id.Value))
                .WithMessage("A categoria informada não está cadastrada. Verifique o ID da categoria.");
            */
        }

        private async Task<bool> EntityExistsAsync<T>(IRepository<T> repository, int id)
        {
            var entity = await repository.GetByIdAsync(id);
            return entity != null; 
        }
    }
}
