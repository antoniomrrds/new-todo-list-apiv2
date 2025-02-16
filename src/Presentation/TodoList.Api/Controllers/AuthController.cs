using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TodoList.Application.DTOs.Auth;
using TodoList.Application.ports.Repositories;
using TodoList.Domain.Entities;
using TodoList.Domain.Enums;
using TodoList.Infrastructure.Security;

namespace TodoList.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthRepository _authRepository;
    private readonly IMapper _mapper;
    private readonly IRoleRepository _roleRepository;

    public AuthController(IAuthRepository authRepository, IMapper mapper, IRoleRepository roleRepository)
    {
        _authRepository = authRepository;
        _roleRepository = roleRepository;
        _mapper = mapper;
    }

    [HttpPost("sign-up")]
    public async Task<ActionResult> SignUp(SignUpDTo signUpDTo)
    {
         var bcryptAdapter = new BcryptAdapter();
         var passwordHasher =  bcryptAdapter.Hash(signUpDTo.Password);
         signUpDTo.Password = passwordHasher;
         var user = _mapper.Map<User>(signUpDTo);
         await _authRepository.SignUpUserAsync(user);
         return NoContent();
    }

    [HttpGet("sign-in")]
    public async Task<ActionResult> SignIn(SignInDTo signInDTo)
    {

       var  role  =  await _roleRepository.RoleExists(Roles.Admin);
         return Ok(role);
    }

}
