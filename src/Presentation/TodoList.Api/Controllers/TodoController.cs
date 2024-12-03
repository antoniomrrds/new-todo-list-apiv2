using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TodoList.Application.DTOs.Todo;
using TodoList.Application.ports.Repositories;
using TodoList.Domain.Constants;

namespace TodoList.Api.Controllers
{
    [ApiController]
    [Route("api/todo")]

    public class TodoController(ITodoRepository todoRepository, IMapper mapper) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoDTo>>> GetTodosAsync()
        {
            var todos = await todoRepository.GetAllAsync();
            var todosResult = todos.Select(mapper.Map<TodoDTo>).ToList();
            return Ok(todosResult);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<ActionResult<TodoDTo>> GetTodoById(int id)
        {
            var todo = await todoRepository.GetByIdAsync(id);
            if (todo.Id == DefaultValues.IdNullValue)  return NotFound();
            var todoResult = mapper.Map<TodoDTo>(todo);
            return Ok(todoResult);
        }
            
        [HttpGet]
        [Route("tagsandcategories/{id:int}")]
        public async Task<ActionResult<TodoWithTagsAndCategoriesDTo>> GetTodoWithTagsAndCategories(int id)
        {
            var result = await todoRepository.GetTodoWithTagsAndCategoriesAsync(id);
            return result.Id == DefaultValues.IdNullValue ? NotFound(): Ok(result);
        }
         
        [HttpPost]
        public async Task<ActionResult> PostAsync(CreateTodoDTo createTodoDTo) 
        {
            var createdId = await todoRepository.CreateAsync(createTodoDTo);
            
            return CreatedAtAction(nameof(GetTodoWithTagsAndCategories), new { id = createdId }, null);
        }
            
        [HttpPut("{id:int}")]
        public async Task<ActionResult> PutAsync(int id, UpdateTodoDTo updateTodoDTo) 
        {
            var existTodo = await todoRepository.GetTodoWithTagAndCategoryIdsAsync(id);
            if (existTodo.Id == DefaultValues.IdNullValue) return NotFound();
            mapper.Map(updateTodoDTo, existTodo);
            existTodo.Id = id;
            var updatedTodo = mapper.Map<UpdateTodoDTo>(existTodo);
            await todoRepository.UpdateAsync(updatedTodo);
            return Ok();
        }


        [HttpDelete]
        [Route("{id:int}")]
        public async Task<ActionResult> DeleteTodoAsync(int id)
        {
            var todo = await todoRepository.GetByIdAsync(id);
            if (todo.Id == DefaultValues.IdNullValue) return NotFound();
            await todoRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
