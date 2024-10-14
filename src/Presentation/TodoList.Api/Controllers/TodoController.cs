using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TodoList.Application.DTOs.Todo;
using TodoList.Domain.Entities;
using TodoList.Application.ports.Repositories;

namespace TodoList.Api.Controllers
{
    [ApiController]
    [Route("api/todo")]

    public class TodoController : ControllerBase
    {
        private readonly ITodoRepository _todoRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IMapper _mapper;

        public TodoController(ITodoRepository todoRepository, IMapper mapper,ITagRepository tagRepository)
        {
            _todoRepository = todoRepository;
            _tagRepository = tagRepository;
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

        [HttpPost]
        public async Task<ActionResult<TodoDTo>> PostAsync(CreateTodoDTo createTodoDTo) 
        {
            var todo = _mapper.Map<Todo>(createTodoDTo);
            var createdId = await _todoRepository.CreateAsync(todo);
            var createdTodoDTo = _mapper.Map<TodoDTo>(todo) with { Id = createdId };
            return CreatedAtAction(
                actionName: nameof(GetTodoById),
                routeValues: new { id = createdId },
                value: createdTodoDTo
            );
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TodoDTo>> PutAsync(int id, CreateTodoDTo createTodoDTo) 
        {
            var existTodo = await _todoRepository.GetByIdAsync(id);
            if (existTodo is null)
            {
                return NotFound();
            }
            
            
            _mapper.Map(createTodoDTo, existTodo);
            await _todoRepository.UpdateAsync(existTodo);
            
            var tagResponse = _mapper.Map<TodoDTo>(existTodo);
            return Ok(tagResponse);
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
