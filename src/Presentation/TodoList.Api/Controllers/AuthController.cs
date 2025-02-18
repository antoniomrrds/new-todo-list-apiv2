using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TodoList.Application.DTOs.Auth;
using TodoList.Application.ports.Repositories;
using TodoList.Application.Ports.Security;
using TodoList.Domain.Constants;
using TodoList.Domain.Entities;
using TodoList.Infrastructure.Security;

namespace TodoList.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthRepository _authRepository;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly ITokenGenerator _tokenGenerator;

    public AuthController(IAuthRepository authRepository,
                          IMapper mapper,
                          IUserRepository userRepository,
                          ITokenGenerator tokenGenerator)
    {
        _authRepository = authRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _tokenGenerator = tokenGenerator;
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

    [HttpPost("sign-in")]
    public async Task<ActionResult<SignInResponseDTo>> SignIn(SignInDTo signInDTo)
    {
        var user = await _userRepository.GetUserByEmailAsync(signInDTo.Email);
        if (user.Id == DefaultValues.IdNullValue)
        {
            return BadRequest("E-mail informado não encontrado.");
        }

        var bcryptAdapter = new BcryptAdapter();
        var passwordIsValid = bcryptAdapter.Verify(signInDTo.Password, user.Password);
        if (!passwordIsValid)
        {
            return BadRequest("Senha inválida.");
        }

        var userRoles = await _userRepository.GetUserRolesAsync(user.Id);
        var token = _tokenGenerator.Generate(userRoles);
        var refreshToken = _tokenGenerator.GenerateRefreshToken(userRoles);
        return Ok(new SignInResponseDTo(token , refreshToken));
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<SignInResponseDTo>> RefreshToken(RefreshTokenDTo refreshTokenDTo)
    {
        var (isValid ,idUser) = await _tokenGenerator.ValidateToken(refreshTokenDTo.RefreshToken);
        if (isValid == false)
            return Unauthorized();

        var userRoles = await _userRepository.GetUserRolesAsync(idUser);
        var token = _tokenGenerator.Generate(userRoles);
        var refreshToken = _tokenGenerator.GenerateRefreshToken(userRoles);
        return Ok(new SignInResponseDTo(token , refreshToken));
    }
}
