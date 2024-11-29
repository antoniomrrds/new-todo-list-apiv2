namespace TodoList.Application.ports.Repositories;

public interface IEntityExistenceChecker
{
    Task<IEnumerable<int>> AreAllEntitiesPresentAsync(IEnumerable<int> ids);
}
