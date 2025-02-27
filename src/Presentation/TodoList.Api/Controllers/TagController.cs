using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoList.Application.DTOs.Shared;
using TodoList.Application.DTOs.Tag;
using TodoList.Application.Helpers;
using TodoList.Application.ports.Repositories;
using TodoList.Domain.Constants;
using TodoList.Domain.Entities;
using TodoList.Domain.Enums;
using TodoList.Infrastructure.Helpers;

namespace TodoList.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TagController(ITagRepository tagRepository, IMapper mapper) : ControllerBase
{

  [HttpGet("{id:int}")]
    public async Task<ActionResult<Tag>> GetId(int id)
    {
        var tag = await tagRepository.GetByIdAsync(id);

        if (tag.Id == DefaultValues.IdNullValue)
        {
            return NotFound();
        }
        return Ok(tag);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Tag>>> GetAllAsync()
    {
        var tags = await tagRepository.GetAllAsync();
        return Ok(tags);
    }

    [CheckRoles(Roles.Admin)]
    [HttpPost("do-filter")]
    public async Task<ActionResult<PagedResultDTo<Tag>>> DoFilter(TagFilterDTo filter)
    {
        var pagination = PaginationHelper.CalculatePagination(filter.Page, filter.PageSize);
        var (tags, totalItems) = await tagRepository.FindByFilter(filter, pagination.Start,
            pagination.PageSize);
        var model = PaginationHelper.CreatePagedResult(tags, totalItems, pagination);
        Console.WriteLine("entrei aqui");

        return Ok(model);
    }

    [HttpPost]
    [CheckRoles(Roles.Admin)]
    public async Task<ActionResult<Tag>> CreateAsync(CreateTagDTo createTagDTo)
    {
        var tag = mapper.Map<Tag>(createTagDTo);
        var createdId = await tagRepository.CreateAsync(tag);
        return CreatedAtAction(
            actionName: nameof(GetId),
            routeValues: new { id = createdId },
            value: null
        );
    }

    [HttpPut("{id:int}")]
    [CheckRoles(Roles.Admin)]
    public async Task<ActionResult<Tag>> UpdateAsync(int id , UpdateTagDTo updateTagDTo)
    {
        var existTag = await tagRepository.GetByIdAsync(id);
        if (existTag.Id == DefaultValues.IdNullValue)
        {
            return NotFound();
        }

        mapper.Map(updateTagDTo, existTag);
       await tagRepository.UpdateAsync(existTag);

        return Ok();
    }

    [HttpDelete("{id:int}")]
    [CheckRoles(Roles.Admin)]
    public async Task<ActionResult> DeleteAsync(int id)
    {
        var existTag = await tagRepository.GetByIdAsync(id);
        if (existTag.Id == DefaultValues.IdNullValue)
        {
            return NotFound();
        }
        await tagRepository.DeleteTagByIdAsync(id);
        return NoContent();
    }
}
