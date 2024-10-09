using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TodoList.Application.ports.Repositories;
using TodoList.Application.DTOs.Tag;
using TodoList.Domain.Entities;

namespace TodoList.Api.Controllers;

[ApiController]
[Route("api/tag")]
public class TagController : ControllerBase
{
    private readonly ITagRepository _tagRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<TagCreateDTo> _createValidator;
    private readonly IValidator<TagUpdateDTo> _updateValidator;

    public TagController(ITagRepository tagRepository, IMapper mapper,
        IValidator<TagCreateDTo> createValidator,
        IValidator<TagUpdateDTo> updateValidator)
    {
        _tagRepository = tagRepository;
        _mapper = mapper;
        _createValidator = _createValidator;
        _updateValidator = _updateValidator;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<TagDTo>>> GetByIdAsync(int id)
    {
        var tag = await _tagRepository.GetByIdAsync(id);
        if (tag is null)
        {
            return NotFound(new ApiResponse<string>
            {
                Errors = new[] { $"Nenhuma tag encontrada com o id: {id} informado." }
            });
        }

        return Ok(new ApiResponse<TagDTo> { Data = tag });
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<TagDTo>>>> GetAllAsync()
    {
        var tags = await _tagRepository.GetAllAsync();
        return Ok(new ApiResponse<IEnumerable<TagDTo>> { Data = tags });
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<TagCreateDTo>>> CreateAsync(TagCreateDTo tagCreateDto)
    {
        var validationResult = await _createValidator.ValidateAsync(tagCreateDto);
        if (!validationResult.IsValid)
        {
            return BadRequest(new ApiResponse<TagCreateDTo>
            {
                Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
            });
        }

        var createdId = await _tagRepository.CreateAsync(tagCreateDto);
        var response = new ApiResponse<TagCreateDTo> { Data = tagCreateDto };

        return CreatedAtAction(
            actionName: nameof(GetByIdAsync),
            routeValues: new { id = createdId },
            value: response
        );
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<TagUpdateDTo>>> UpdateAsync(int id, TagUpdateDTo tagUpdateDto)
    {
        var existingTag = await _tagRepository.GetByIdAsync(id);
        if (existingTag is null)
        {
            return NotFound(new ApiResponse<string>
            {
                Errors = new[] { $"Nenhuma tag encontrada com o id: {id} informado." }
            });
        }

        var validationResult = await _updateValidator.ValidateAsync(tagUpdateDto);
        if (!validationResult.IsValid)
        {
            return BadRequest(new ApiResponse<TagUpdateDTo>
            {
                Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
            });
        }

        await _tagRepository.UpdateAsync(tagUpdateDto);
        return Ok(new ApiResponse<TagUpdateDTo> { Data = tagUpdateDto });
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<string>>> DeleteAsync(int id)
    {
        var tag = await _tagRepository.GetByIdAsync(id);
        if (tag is null)
        {
            return NotFound(new ApiResponse<string>
            {
                Errors = new[] { $"Nenhuma tag encontrada com o id: {id} informado." }
            });
        }

        await _tagRepository.DeleteAsync(id);
        return NoContent();
    }


    public class ApiResponse<T>
    {
        public T Data { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}