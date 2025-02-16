using System.ComponentModel;

namespace TodoList.Domain.Enums;

public enum Roles
{
    [Description("Usuário")]
    User = 0,

    [Description("Administrador")]
    Admin = 1

}
