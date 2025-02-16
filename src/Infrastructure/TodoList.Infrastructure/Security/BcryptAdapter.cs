using TodoList.Application.Ports.Security;

namespace TodoList.Infrastructure.Security;

public class BcryptAdapter: IHasher<string>
{
    public string Hash(string input)
    {
        return BCrypt.Net.BCrypt.HashPassword(input);
    }

    public bool Verify(string input, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(input, hash);
    }
}
