namespace DndManager.Data;

public interface IRepository<T> where T : class
{
    Task<T> GetByIdAsync(object id);

    Task<bool> ExistsAsync(object id);

    IList<T> GetAll();

    Task<T> AddAsync(T entity);

    Task Delete(object id);

    void Delete(T entityToDelete);

    void Update(T entityToUpdate);
}
