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
    private readonly IValidator<TagCreateDTo> _createValidator;
    private readonly IMapper _mapper;

    public TagController(ITagRepository tagRepository,
        IValidator<TagCreateDTo> createValidator,
        IMapper mapper)
    {
        _tagRepository = tagRepository;
        _createValidator = createValidator;
        _mapper = mapper;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<TagDTo>>> GetId(int id)
    {
        var tag = await _tagRepository.GetByIdAsync(id);
        if (tag is null)
        {
            return NotFound(new ApiResponse<string>
            {
                Errors = new[] { $"Nenhuma tag encontrada com o id: {id} informado." }
            });
        }

        var tagResult = _mapper.Map<TagDTo>(tag);

        return Ok(new ApiResponse<TagDTo> { Data = tagResult });
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<TagDTo>>>> GetAllAsync()
    {
        var tags = await _tagRepository.GetAllTagsWithDetailsAsync();
        var tagsResult = tags.Select(tag => _mapper.Map<TagDTo>(tag)).ToList();
        return Ok(new ApiResponse<IEnumerable<TagDTo>> { Data = tagsResult });
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

        var tag = _mapper.Map<Tag>(tagCreateDto);
        var createdId = await _tagRepository.CreateAsync(tag);
        var response = new ApiResponse<TagCreateDTo> { Data = tagCreateDto };

        return CreatedAtAction(
            actionName: nameof(GetId),
            routeValues: new { id = createdId },
            value: response
        );
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<TagDTo>>> UpdateAsync(int id, TagCreateDTo tagUpdateDto)
    {
        var existingTag = await _tagRepository.GetByIdAsync(id);
        if (existingTag is null)
        {
            return NotFound(new ApiResponse<string>
            {
                Errors = new[] { $"Nenhuma tag encontrada com o id: {id} informado." },
                Code = StatusCodes.Status404NotFound
            });
        }

        var validationResult = await _createValidator.ValidateAsync(tagUpdateDto);

        if (!validationResult.IsValid)
        {
            return BadRequest(new ApiResponse<TagCreateDTo>
            {
                Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray(),
                Code = StatusCodes.Status400BadRequest
            });
        }

        _mapper.Map(tagUpdateDto, existingTag);

        await _tagRepository.UpdateAsync(existingTag);
        var tagResponse = _mapper.Map<TagDTo>(existingTag);
        return Ok(new ApiResponse<TagDTo>
        {
            Data = tagResponse,
            Code = StatusCodes.Status200OK,
            Errors = Array.Empty<string>()
        });
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

        await _tagRepository.DeleteTagByIdAsync(id);
        return NoContent();
    }


    
    
    // private ApiResponse<T> CreateErrorResponse<T>(T errors, int statusCode)
    // {
    //     return new ApiResponse<T>
    //     {
    //         Errors = errors is string ? new[] { errors as string } : (IEnumerable<string>)errors,
    //         Code = statusCode
    //     };
    // }
    //
    // private ApiResponse<T> CreateSuccessResponse<T>(T data, int statusCode)
    // {
    //     return new ApiResponse<T>
    //     {
    //         Data = data,
    //         Code = statusCode,
    //         Errors = Array.Empty<string>()
    //     };
    // }
    //
    public class ApiResponse<T>
    {
        public T? Data { get; set; }
        public int Code { get; set; }
        public IEnumerable<string>? Errors { get; set; }
    }
}