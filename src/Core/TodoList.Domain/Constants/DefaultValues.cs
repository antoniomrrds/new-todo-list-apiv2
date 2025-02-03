using System.ComponentModel;

namespace TodoList.Domain.Constants;

public static class DefaultValues
{
    public const int Active = 1;
    public const int Inactive = 0;
    public const int IdNullValue = 0;
}

public enum TodoStatus
{
    [Description("Em andamento")]
    InProgress = 1,

    [Description("Expirado")]
    Expired = 2,

    [Description("Indeterminado")]
    NoValidity = 3,

    [Description("Concluído")]
    Completed = 4,

    [Description("Suspenso")]
    Suspended = 0,

    [Description("Status")]
    Unfiltered = -1
}



