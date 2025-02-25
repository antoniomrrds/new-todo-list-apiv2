using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoList.Application.DTOs.Shared;
using TodoList.Application.DTOs.Todo;
using TodoList.Application.Helpers;
using TodoList.Application.ports.Repositories;
using TodoList.Domain.Constants;
using TodoList.Domain.Entities;
using TodoList.Domain.Enums;
using TodoList.Domain.extensions;
using TodoList.Domain.Extensions;
using TodoList.Infrastructure.Helpers;

namespace TodoList.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/todo")]
public class TodoController(ITodoRepository todoRepository, IMapper mapper)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Todo>>> GetTodosAsync()
    {
        var todos = await todoRepository.GetAllAsync();
        var todosResult = todos.ToList();
        return Ok(todosResult.Take(100));
    }

    [HttpPost("doFilter")]
    public async Task<PagedResultDTo<Todo>> DoFilter(ToDoFilterDTo filter)
    {
        var pagination =
            PaginationHelper.CalculatePagination(filter.Page,
                filter.PageSize);
        filter.IdUser = User.GetId();
        var (todos, totalItems) = await todoRepository.FindByFilter(filter,
            pagination.Start, pagination.PageSize);
        var model =
            PaginationHelper.CreatePagedResult(todos, totalItems,
                pagination);

        return model;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Todo>> GetTodoById(int id)
    {
        var todo = await todoRepository.GetByIdAsync(id);
        if (todo.Id == DefaultValues.IdNullValue) return NotFound();
        return Ok(todo);
    }

    [HttpGet("tagsandcategories/{id:int}")]
    public async Task<ActionResult<TodoWithTagsAndCategoriesDTo>>
        GetTodoWithTagsAndCategories(int id)
    {
        var result =
            await todoRepository.GetTodoWithTagsAndCategoriesAsync(id);

        if(result.Id == DefaultValues.IdNullValue) return NotFound();


        return  Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult> PostAsync(CreateTodoDTo createTodoDTo)
    {
        createTodoDTo.IdUser = User.GetId();
        var createdId = await todoRepository.CreateAsync(createTodoDTo);

        return CreatedAtAction(nameof(GetTodoWithTagsAndCategories),
            new { id = createdId }, null);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> PutAsync(int id,
        UpdateTodoDTo updateTodoDTo)
    {
        var existTodo =
            await todoRepository.GetTodoWithTagAndCategoryIdsAsync(id);
        if (existTodo.Id == DefaultValues.IdNullValue) return NotFound();
        mapper.Map(updateTodoDTo, existTodo);
        existTodo.Id = id;
        var updatedTodo = mapper.Map<UpdateTodoDTo>(existTodo);
        await todoRepository.UpdateAsync(updatedTodo);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteTodoAsync(int id)
    {
        var todo = await todoRepository.GetByIdAsync(id);
        if (todo.Id == DefaultValues.IdNullValue) return NotFound();
        await todoRepository.DeleteAsync(id);
        return NoContent();
    }
}
