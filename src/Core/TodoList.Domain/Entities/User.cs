using TodoList.Domain.Constants;
using TodoList.Domain.Enums;

namespace TodoList.Domain.Entities;

public class User: BaseEntity
{
    public string Name { get;  set; } = string.Empty;
    public string Email { get; } = string.Empty;
    public string Password { get; private set; } = string.Empty;
    public ActivationState Active { get; set; }
    public Guid IdImage { get; set; }
    public string UrlImage { get;  } = string.Empty;
   public string CreatedAtFormatted => CreatedAt.ToString("dd/MM/yyyy HH:mm:ss");
    public string UpdatedAtFormatted => UpdatedAt.ToString("dd/MM/yyyy HH:mm:ss");

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

    private void SetCreationDate()
    {
        CreatedAt = DateTime.Now;
    }


    private void SetUpdateDate()
    {
        UpdatedAt = DateTime.Now;
    }

    public void SetCreateAndUpdateDate()
    {
        if (Id == DefaultValues.IdNullValue)
        {
            SetCreationDate();
            SetUpdateDate();
        }
        SetUpdateDate();

    }
}
