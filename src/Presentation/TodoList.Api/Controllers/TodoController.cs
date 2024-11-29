using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TodoList.Application.DTOs.Todo;
using TodoList.Application.ports.Repositories;

namespace TodoList.Api.Controllers
{
    [ApiController]
    [Route("api/todo")]

    public class TodoController : ControllerBase
    {
        private readonly ITodoRepository _todoRepository;
        private readonly IMapper _mapper;

        public TodoController(ITodoRepository todoRepository, IMapper mapper)
        {
            _todoRepository = todoRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoDTo>>> GetTodosAsync()
        {
            var todos = await _todoRepository.GetAllAsync();
            var todosResult = todos.Select(todo => _mapper.Map<TodoDTo>(todo)).ToList();
            return Ok(todosResult);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<TodoDTo>> GetTodoById(int id)
        {
            var todo = await _todoRepository.GetByIdAsync(id);
            if (todo is null)
            {
                return NotFound();
            }

            var todoResult = _mapper.Map<TodoDTo>(todo);
            return Ok(todoResult);
        }

        [HttpGet]
        [Route("tagsandcategories/{id}")]
        public async Task<ActionResult<TodoWithTagsAndCategoriesDTo>> GetTodoWithTagsAndCategoriesAsync(int id)
        {
            var result = await _todoRepository.GetTodoWithTagsAndCategoriesAsync(id);
            return result is null ? NotFound(): Ok(result);
        }
        
        [HttpPost]
        public async Task<ActionResult> PostAsync(CreateTodoDTo createTodoDTo) 
        {
            var createdId = await _todoRepository.CreateAsync(createTodoDTo);
            
            return CreatedAtAction(nameof(GetTodoById), new { id = createdId }, null);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> PutAsync(int id, UpdateTodoDTo updateTodoDTo) 
        {
            var existTodo = await _todoRepository.GetTodoWithTagAndCategoryIdsAsync(id);
            if (existTodo is null)
            {
                return NotFound();
            }
            
            _mapper.Map(updateTodoDTo, existTodo);
            existTodo = existTodo with { Id = id };
            var updatedTodo = _mapper.Map<UpdateTodoDTo>(existTodo);
            
            await _todoRepository.UpdateAsync(updatedTodo);
            return Ok();
        }


        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> DeleteTodoAsync(int id)
        {
            var todo = await _todoRepository.GetByIdAsync(id);
            if (todo is null)
            {
                return NotFound();
            }

            await _todoRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
