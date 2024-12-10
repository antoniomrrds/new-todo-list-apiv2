namespace TodoList.Application.ports.Repositories;

public interface IEntityExistenceCheckerRepository
{
    Task<IEnumerable<int>> AreAllEntitiesPresentAsync(IEnumerable<int> ids);
}
