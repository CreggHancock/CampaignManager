using Microsoft.EntityFrameworkCore;

namespace DndManager.Data;

public interface IRepository<T> where T : class
{
    Task<T> GetByIdAsync(object id);

    Task<bool> ExistsAsync(object id);

    Task<IList<T>> GetAllAsync();

    Task<IList<T>> GetAsync(Func<DbSet<T>, Task<IList<T>>> func);

    Task<T> GetAsync(Func<DbSet<T>, Task<T>> func);

    Task<T> AddAsync(T entity);

    Task DeleteAsync(object id);

    void Delete(T entityToDelete);

    void Update(T entityToUpdate);
}
