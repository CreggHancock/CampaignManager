namespace DndManager.Data;

public interface IUnitOfWork : IDisposable
{
    void SaveChanges();

    Task SaveChangesAsync();

    void Rollback();

    IRepository<T> Repository<T>() where T : class;
}
