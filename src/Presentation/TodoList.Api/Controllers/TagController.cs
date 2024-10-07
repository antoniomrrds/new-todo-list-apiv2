using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TodoList.Application.ports.Repositories;
using TodoList.Application.DTOs;
using TodoList.Domain.Entities;

namespace TodoList.Api.Controllers;
[ApiController]
[Route("api/tag")]
public class TagController : ControllerBase
{
    private readonly ITagRepository _tagRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<TagDTo> _validator;
    
    public TagController(ITagRepository tagRepository, IMapper mapper, IValidator<TagDTo> validator)
    {
        _tagRepository = tagRepository;
        _mapper = mapper;
        _validator = validator;
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var tag = await _tagRepository.GetByIdAsync(id);
        if (tag is null)
        {
            return NotFound(new
            {
                result = new
                {
                    error = "Nenhuma tag encontrada com o id informado."
                }
            });
        }
        var tagDto = _mapper.Map<Tag>(tag);
        return Ok(tagDto);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateAsync(TagDTo tagDTo)
    {
        var validationResult = await _validator.ValidateAsync(tagDTo);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        var tag = _mapper.Map<Tag>(tagDTo);
        await _tagRepository.CreateAsync(tag);
        return Ok();
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id,TagDTo tagDTo)
    {
        var validationResult = await _validator.ValidateAsync(tagDTo);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var tag = await _tagRepository.GetByIdAsync(id);
        if (tag is null)
        {
            return NotFound(new
            {
                result = new
                {
                    error = "Nenhuma tag encontrada com o id informado."
                }
            });
        }
        var tagToUpdate = _mapper.Map<Tag>(tagDTo);
        tagToUpdate.Id = id;
        var updated = await _tagRepository.UpdateAsync(tag);
        return Ok(updated);
    }
}