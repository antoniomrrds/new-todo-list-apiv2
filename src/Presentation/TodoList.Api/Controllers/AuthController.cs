using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoList.Api.Helpers;
using TodoList.Application.DTOs.Auth;
using TodoList.Application.Factories;
using TodoList.Application.ports.Repositories;
using TodoList.Application.Ports.Security;
using TodoList.Domain.Constants;
using TodoList.Domain.Entities;


namespace TodoList.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthRepository _authRepository;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IHasher<string> _hasher;

    public AuthController(IAuthRepository authRepository,
                          IMapper mapper,
                          IUserRepository userRepository,
                          ITokenGenerator tokenGenerator,
                          IHasher<string> hasher)
    {
        _authRepository = authRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _tokenGenerator = tokenGenerator;
        _hasher = hasher;
    }

    [HttpPost("sign-up")]
    public async Task<ActionResult> SignUp(SignUpDTo signUpDTo)
    {

        var passwordHasher = _hasher.Hash(signUpDTo.Password);
        signUpDTo.Password = passwordHasher;
        var user = _mapper.Map<User>(signUpDTo);
        await _authRepository.SignUpUserAsync(user);
        return NoContent();
    }

    [HttpPost("sign-in")]
    public async Task<ActionResult> SignIn(SignInDTo signInDTo)
    {
        var user = await _userRepository.GetUserByEmailAsync(signInDTo.Email);
        if (user.Id == DefaultValues.IdNullValue)
        {
            var errors = new Dictionary<string, List<string>>
            {
                { "Email", ["Usuário não encontrado."] }
            };

            return BadRequestResponseFactory.CreateBadRequestResponse("A solicitação contém erros de validação.", errors);
        }

        var passwordIsValid = _hasher.Verify(signInDTo.Password, user.Password);
        if (!passwordIsValid)
        {
            var errors = new Dictionary<string, List<string>>
            {
                { "Senha", ["Senha incorreta."] }
            };
            return BadRequestResponseFactory.CreateBadRequestResponse("A solicitação contém erros de validação.", errors);
        }

        var userRoles = await _userRepository.GetUserRolesAsync(user.Id);
        var token = _tokenGenerator.Generate(userRoles);
        var refreshToken = _tokenGenerator.GenerateRefreshToken(userRoles);

        // Usando o CookieHelper para definir os cookies
        var roles  = userRoles.Roles.Select(r => r.RoleType).ToArray();

        var sessionData = new SessionData(user.Name, user.Email, roles);
        CookieHelper.SetAuthCookies(Response, token, refreshToken, sessionData);

        return Ok();
    }
    [Authorize]
    [HttpGet("is-logged-in")]
    public IActionResult IsLoggedIn() =>  NoContent();

    [HttpGet("sign-out")]
    public IActionResult LogOut()
    {
        CookieHelper.ClearCookiesAuth(Response);
        return NoContent();
    }

}
