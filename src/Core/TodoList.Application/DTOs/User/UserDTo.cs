using TodoList.Domain.Enums;

namespace TodoList.Application.DTOs.User;

public record UserNameRequestDTo(string Name);

public record UserResponseWithoutPasswordDTo
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public Guid IdImage { get;init; }
    public string UrlImage { get; init; } = string.Empty;
    public ActivationState Active { get; init; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string CreatedAtFormatted => CreatedAt.ToString("dd/MM/yyyy HH:mm:ss");
    public string UpdatedAtFormatted => UpdatedAt.ToString("dd/MM/yyyy HH:mm:ss");
}
