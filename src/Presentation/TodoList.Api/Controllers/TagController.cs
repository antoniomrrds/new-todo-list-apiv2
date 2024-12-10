using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TodoList.Application.DTOs.Tag;
using TodoList.Application.ports.Repositories;
using TodoList.Domain.Constants;
using TodoList.Domain.Entities;

namespace TodoList.Api.Controllers;

[ApiController]
[Route("api/tag")]
public class TagController(ITagRepository tagRepository, IMapper mapper) : ControllerBase
{
  [HttpGet("{id:int}")]
    public async Task<ActionResult<TagDTo>> GetId(int id)
    {
        var tag = await tagRepository.GetByIdAsync(id);
        
        if (tag.Id == DefaultValues.IdNullValue)
        {
            return NotFound();
        }

        var tagResult = mapper.Map<TagDTo>(tag);
        return Ok(tagResult);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TagDTo>>> GetAllAsync()
    {
        var tags = await tagRepository.GetAllTagsWithDetailsAsync();
        var tagsResult = tags.Select(tag => mapper.Map<TagDTo>(tag)).ToList();
        return Ok(tagsResult);
    }

    [HttpPost]
    public async Task<ActionResult<TagDTo>> CreateAsync(
        [FromBody]
        CreateTagDTo createTagDTo)
    {
        var tag = mapper.Map<Tag>(createTagDTo);
        var createdId = await tagRepository.CreateAsync(tag);

        var createdTagDTo = mapper.Map<TagDTo>(tag) with { Id = createdId };
        return CreatedAtAction(
            actionName: nameof(GetId),
            routeValues: new { id = createdId },
            value: createdTagDTo
        );
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TagDTo>> UpdateAsync(int id, CreateTagDTo updateTagDTo)
    {
        var existTag = await tagRepository.GetByIdAsync(id);
        if (existTag is null)
        {
            return NotFound();
        }

        mapper.Map(updateTagDTo, existTag); 
       await tagRepository.UpdateAsync(existTag);

        var tagResponse = mapper.Map<TagDTo>(existTag);
        return Ok(tagResponse);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteAsync(int id)
    {
        var tag = await tagRepository.GetByIdAsync(id);
        if (tag is null)
        {
            return NotFound();
        }

        await tagRepository.DeleteTagByIdAsync(id);
        return NoContent();
    }
}
