namespace TodoList.Application.DTOs.Auth;

public record SignInDTo
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public record SignInResponseDTo(string Token, string RefreshToken);


