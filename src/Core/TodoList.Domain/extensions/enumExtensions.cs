using System.ComponentModel;
using System.Reflection;

namespace TodoList.Domain.Extensions
{
    public static class EnumExtensions
    {
        public static int ToInt(this Enum value) => Convert.ToInt32(value);

        public static string GetDescription<TEnum>(this TEnum value) where TEnum : Enum
        {
            var field = typeof(TEnum).GetField(value.ToString());
            var attribute = field?.GetCustomAttribute<DescriptionAttribute>();

            return attribute?.Description ?? value.ToString();
        }
    }
}
