using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TodoList.Application.DTOs.Category;
using TodoList.Application.ports.Repositories;
using TodoList.Domain.Constants;
using TodoList.Domain.Entities;

namespace TodoList.Api.Controllers;

[ApiController]
[Route("api/category")]
public class CategoryController(ICategoryRepository categoryRepository, IMapper mapper) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Category>> GetId(int id)
    {
        var category = await categoryRepository.GetByIdAsync(id);
        if (category.Id == DefaultValues.IdNullValue)
        {
            return NotFound();
        }

        return Ok(category);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetAllAsync()
    {
        var categories = await categoryRepository.GetAllCategoriesWithDetailsAsync();
        return Ok(categories);
    }

    [HttpPost]
    public async Task<ActionResult<Category>> CreateAsync(CreateCategoryDTo category)
    {
        var categoryEntity = mapper.Map<Category>(category);
        var id = await categoryRepository.CreateAsync(categoryEntity);
        return CreatedAtAction(nameof(GetId), new { id  }, null);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Category>> UpdateAsync(int id,UpdateCategoryDTo updateCategoryDTo)
    {
        var exist = await categoryRepository.GetByIdAsync(id);
        if (exist.Id == DefaultValues.IdNullValue)
        {
            return NotFound();
        } 
        mapper.Map(updateCategoryDTo, exist);
        await categoryRepository.UpdateAsync(exist);
        return Ok();
    }
    
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<Category>> DeleteAsync(int id)
    {
        var exist = await categoryRepository.GetByIdAsync(id);
        if (exist.Id == DefaultValues.IdNullValue)
        {
            return NotFound();
        } 
        await categoryRepository.DeleteCategoryByIdAsync(id);
        return NoContent();
    }
    
}