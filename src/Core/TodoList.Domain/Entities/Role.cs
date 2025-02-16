using TodoList.Domain.Constants;
using TodoList.Domain.Enums;

namespace TodoList.Domain.Entities;

public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Roles RoleType { get; set; }


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
