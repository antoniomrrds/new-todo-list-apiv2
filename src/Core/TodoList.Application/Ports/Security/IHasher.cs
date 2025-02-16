namespace TodoList.Application.Ports.Security;

public interface IHasher<in T>
{
    string Hash(T input);
    bool Verify(T input, string hash);
}
