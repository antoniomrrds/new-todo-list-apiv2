using TodoList.Domain.Constants;
using TodoList.Domain.Enums;

namespace TodoList.Domain.Entities;

public class User: BaseEntity
{
    public string Name { get;  set; } = string.Empty;
    public string Email { get; } = string.Empty;
    public string Password { get; private set; } = string.Empty;
    public ActivationState Active { get; set; }

    public User ()
    {
        Active = ActivationState.Active;
    }

    public User(string name, string email, string password): this()
    {
        Name = name;
        Email = email;
        Password = password;
    }

    public void SetCreationDate()
    {
        CreatedAt = DateTime.Now;
    }


    public void SetUpdateDate()
    {
        UpdatedAt = DateTime.Now;
    }

    public void SetCreateAndUpdateDate()
    {
        if (this.Id == DefaultValues.IdNullValue)
        {
            SetCreationDate();
            SetUpdateDate();
        }
        SetUpdateDate();

    }
}
