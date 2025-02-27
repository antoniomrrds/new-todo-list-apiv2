using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoList.Application.DTOs.User;
using TodoList.Application.Factories;
using TodoList.Application.ports.Repositories;
using TodoList.Application.Ports.Security;
using TodoList.Domain.Constants;
using TodoList.Domain.Extensions;


namespace TodoList.Api.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController: ControllerBase
{

    private readonly IUserRepository _userRepository;
    private readonly IHasher<string> _hasher;
    private readonly IMapper _mapper;

    public UserController(IUserRepository userRepository,
                          IMapper mapper,
                          IHasher<string> hasher)
    {
        _userRepository = userRepository;
        _hasher = hasher;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<UserResponseWithoutPasswordDTo>> GetUser()
    {
        var userId = User.GetId();
        var user = await _userRepository.GetUserByIdAsync(userId);

        var userResponse = _mapper.Map<UserResponseWithoutPasswordDTo>(user);
        if (user.Id != DefaultValues.IdNullValue) return Ok(userResponse);
        var errors = new Dictionary<string, List<string>>
        {
            { "Id", ["Usuário não encontrado."] }
        };

        return BadRequestResponseFactory.CreateBadRequestResponse("A solicitação contém erros de validação.",
            errors);
    }
    [HttpPut]
    public async Task<ActionResult<UserResponseWithoutPasswordDTo>> UpdateUser(UserNameRequestDTo userNameRequestDTo)
    {
        var userId = User.GetId();
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user.Id == DefaultValues.IdNullValue)
        {
            var errors = new Dictionary<string, List<string>>
            {
                { "Id", ["Usuário não encontrado."] }
            };

            return BadRequestResponseFactory.CreateBadRequestResponse("A solicitação contém erros de validação.", errors);
        }

        user.Name = userNameRequestDTo.Name;
        var userUpdate = await _userRepository.UpdateUserProfileAsync(user.Id, user.Name);
        return Ok(userUpdate);
    }

    [HttpPost("change-password")]
    public async Task<ActionResult> ChangePassword(ChangePasswordDTo changePasswordDTo)
    {
        var userId = User.GetId();
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user.Id == DefaultValues.IdNullValue)
        {
            var errors = new Dictionary<string, List<string>>
            {
                { "Id", ["Usuário não encontrado."] }
            };

            return BadRequestResponseFactory.CreateBadRequestResponse("A solicitação contém erros de validação.", errors);
        }


        var passwordHasher = _hasher.Hash(changePasswordDTo.Password);
        await _userRepository.ChangePasswordAsync(user.Id, passwordHasher);
        return NoContent();
    }
}
