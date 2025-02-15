using Microsoft.AspNetCore.Mvc;
using TodoList.Application.DTOs.Auth;
using TodoList.Infrastructure.Security;

namespace TodoList.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{

    [HttpPost("register")]
    public ActionResult RegisterUser(RegisterUser registerUser)
    {
         var bcryptAdapter = new BcryptAdapter();
         var passwordHasher =  bcryptAdapter.Hash(registerUser.Password);
        registerUser.Password = passwordHasher;
        return Ok($"User registered: {registerUser}");
    }

}
