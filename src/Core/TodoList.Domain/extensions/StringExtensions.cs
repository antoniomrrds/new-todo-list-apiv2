using System.Text.RegularExpressions;

namespace TodoList.Domain.extensions;

public static partial class StringExtensions
{
    public static bool IsEmail(this string email)
    {
        var re = EmailRegex();
        return re.IsMatch(email);
    }

    [GeneratedRegex(@"^(?![_.])[A-Za-z0-9._%+-]+(?:[A-Za-z0-9.-]*[A-Za-z0-9]+)*@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$", RegexOptions.IgnoreCase, "pt-BR")]
    private static partial Regex EmailRegex();
}
