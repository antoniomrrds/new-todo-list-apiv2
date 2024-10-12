using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TodoList.Api.Filters;
using TodoList.Application.DTOs.Tag;
using TodoList.Application.ports.Repositories;
using TodoList.Domain.Entities;

namespace TodoList.Api.Controllers;

[ApiController]
[Route("api/tag")]
[ServiceFilter(typeof(ValidateModelAttribute))]
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
            return NotFound($"Nenhuma tag encontrada com o id: {id} informado.");
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
    public async Task<ActionResult<TagDTo>> CreateAsync(TagCreateDTo tagCreateDto)
    {
        var tag = _mapper.Map<Tag>(tagCreateDto);
        var createdId = await _tagRepository.CreateAsync(tag);

        var createdTagDto = _mapper.Map<TagDTo>(tag);
        return CreatedAtAction(
            actionName: nameof(GetId),
            routeValues: new { id = createdId },
            value: createdTagDto
        );
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TagDTo>> UpdateAsync(int id, TagCreateDTo tagUpdateDto)
    {
        var existingTag = await _tagRepository.GetByIdAsync(id);
        if (existingTag is null)
        {
            return NotFound();
        }

        _mapper.Map(tagUpdateDto, existingTag);
        await _tagRepository.UpdateAsync(existingTag);

        var tagResponse = _mapper.Map<TagDTo>(existingTag);
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
