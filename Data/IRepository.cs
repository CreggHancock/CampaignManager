namespace DndManager.Data;

public interface IRepository<T> where T : class
{
    Task<T> GetById(object id);
    IList<T> GetAll();
    Task Add(T entity);
}
