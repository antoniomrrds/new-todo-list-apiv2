using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TodoList.Api.Filters;
using TodoList.Application.DTOs.Tag;
using TodoList.Application.ports.Repositories;
using TodoList.Domain.Entities;

namespace TodoList.Api.Controllers;

[ApiController]
[Route("api/tag")]
public class TagController : ControllerBase
{
    private readonly ITagRepository _tagRepository;
    private readonly IMapper _mapper;

    public TagController(ITagRepository tagRepository, IMapper mapper)
    {
        _tagRepository = tagRepository;
        _mapper = mapper;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TagDTo>> GetId(int id)
    {
        var tag = await _tagRepository.GetByIdAsync(id);
        if (tag is null)
        {
            return NotFound();
        }

        var tagResult = _mapper.Map<TagDTo>(tag);
        return Ok(tagResult);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TagDTo>>> GetAllAsync()
    {
        var tags = await _tagRepository.GetAllTagsWithDetailsAsync();
        var tagsResult = tags.Select(tag => _mapper.Map<TagDTo>(tag)).ToList();
        return Ok(tagsResult);
    }

    [HttpPost]
    public async Task<ActionResult<TagDTo>> CreateAsync(
        [FromBody]
        CreateTagDTo createTagDTo)
    {
        var tag = _mapper.Map<Tag>(createTagDTo);
        var createdId = await _tagRepository.CreateAsync(tag);

        var createdTagDTo = _mapper.Map<TagDTo>(tag) with { Id = createdId };
        return CreatedAtAction(
            actionName: nameof(GetId),
            routeValues: new { id = createdId },
            value: createdTagDTo
        );
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TagDTo>> UpdateAsync(int id, CreateTagDTo updateTagDTo)
    {
        var existTag = await _tagRepository.GetByIdAsync(id);
        if (existTag is null)
        {
            return NotFound();
        }

        _mapper.Map(updateTagDTo, existTag); 
       await _tagRepository.UpdateAsync(existTag);

        var tagResponse = _mapper.Map<TagDTo>(existTag);
        return Ok(tagResponse);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync(int id)
    {
        var tag = await _tagRepository.GetByIdAsync(id);
        if (tag is null)
        {
            return NotFound();
        }

        await _tagRepository.DeleteTagByIdAsync(id);
        return NoContent();
    }
}
