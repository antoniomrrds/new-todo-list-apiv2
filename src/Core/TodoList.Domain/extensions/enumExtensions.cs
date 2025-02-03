using System.ComponentModel;
using System.Reflection;

namespace TodoList.Domain.extensions;

public static class EnumExtensions
{
    public static int ToInt(this Enum value) => Convert.ToInt32(value);
    public static string GetName<TEnum>(this TEnum value) where TEnum : Enum
    {
        // Obtém o atributo [Description] caso esteja presente no enum
        var field = typeof(TEnum).GetField(value.ToString());
        var attribute = field?.GetCustomAttribute<DescriptionAttribute>();

        return attribute?.Description ?? value.ToString();
    }
}
