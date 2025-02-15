using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TodoList.Application.DTOs.Auth;
using TodoList.Application.ports.Repositories;
using TodoList.Domain.Entities;
using TodoList.Infrastructure.Security;

namespace TodoList.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthRepository _authRepository;
    private readonly IMapper _mapper;

    public AuthController(IAuthRepository authRepository, IMapper mapper)
    {
        _authRepository = authRepository;
        _mapper = mapper;
    }

    [HttpPost("sign-up")]
    public async Task<ActionResult> SignUp(RegisterUserDTo registerUserDTo)
    {
         var bcryptAdapter = new BcryptAdapter();
         var passwordHasher =  bcryptAdapter.Hash(registerUserDTo.Password);
         registerUserDTo.Password = passwordHasher;
         var user = _mapper.Map<User>(registerUserDTo);
        await _authRepository.SignUpUserAsync(user);
        return NoContent();
    }

}
