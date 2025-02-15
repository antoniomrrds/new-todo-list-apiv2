using TodoList.Domain.Enums;

namespace TodoList.Domain.Entities;

public class User
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public ActivationState Active { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }


}
