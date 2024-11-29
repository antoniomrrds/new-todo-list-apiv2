namespace TodoList.Application.ports.Repositories;

public interface IEntityExistenceChecker
{
    Task<bool> AreAllEntitiesPresentAsync(IEnumerable<int> ids);
}
