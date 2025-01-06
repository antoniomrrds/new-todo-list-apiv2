namespace TodoList.Domain.extensions;

public static class EnumExtensions
{
    public static int ToInt(this Enum value) => Convert.ToInt32(value);
}