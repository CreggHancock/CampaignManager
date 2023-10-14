namespace DndManager.Data;

public interface IRepository<T> where T : class
{
    Task<T> GetById(object id);

    IList<T> GetAll();

    Task<T> AddAsync(T entity);

    Task Delete(object id);

    void Delete(T entityToDelete);

    void Update(T entityToUpdate);
}
