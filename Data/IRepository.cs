using Microsoft.EntityFrameworkCore;

namespace DndManager.Data;

public interface IRepository<T> where T : class
{
    Task<T> GetByIdAsync(object id);

    Task<bool> ExistsAsync(object id);

    Task<IList<T>> GetAll();

    Task<IList<T>> Get(Func<DbSet<T>, Task<IList<T>>> func);

    Task<T> Get(Func<DbSet<T>, Task<T>> func);

    Task<T> AddAsync(T entity);

    Task Delete(object id);

    void Delete(T entityToDelete);

    void Update(T entityToUpdate);
}
