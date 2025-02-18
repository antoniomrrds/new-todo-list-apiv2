using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoList.Application.DTOs.Tag;
using TodoList.Application.ports.Repositories;
using TodoList.Domain.Constants;
using TodoList.Domain.Entities;
using TodoList.Domain.Enums;
using TodoList.Domain.Extensions;
using TodoList.Infrastructure.Helpers;

namespace TodoList.Api.Controllers;


[Authorize]
[ApiController]
[Route("api/tag")]
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
    [CheckRoles(Roles.User)]
    public async Task<ActionResult<IEnumerable<Tag>>> GetAllAsync()
    {
        var tags = await tagRepository.GetAllAsync();
        return Ok(tags);
    }

    [HttpPost]
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
