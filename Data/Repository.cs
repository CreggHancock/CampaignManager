namespace DndManager.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly ApplicationDbContext _dbContext;
    private readonly DbSet<T> _dbSet;
    public Repository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<T>();
    }

    public async Task<T> GetByIdAsync(object id)
    {
        var obj = await _dbSet.FindAsync(id);

        if (obj == null)
        {
            throw new DataException($"Could not get object with id {id}");
        }

        return obj;
    }

    public async Task<bool> ExistsAsync(object id)
    {
        var obj = await _dbSet.FindAsync(id);
        return obj != null;
    }

    public async Task<IList<T>> GetAll()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<IList<T>> Get(Func<DbSet<T>, Task<IList<T>>> func)
    {
        return await func(_dbSet);
    }

    public async Task<T> Get(Func<DbSet<T>, Task<T>> func)
    {
        return await func(_dbSet);
    }

    public async Task<T> AddAsync(T entity)
    {
        var addedEntity = await _dbSet.AddAsync(entity);
        return addedEntity.Entity;
    }

    public virtual async Task Delete(object id)
    {
        var obj = await _dbSet.FindAsync(id);

        if (obj == null)
        {
            throw new DataException($"Could not get object with id {id}");
        }

        Delete(obj);
    }

    public virtual void Delete(T entityToDelete)
    {
        if (_dbContext.Entry(entityToDelete).State == EntityState.Detached)
        {
            _dbSet.Attach(entityToDelete);
        }
        _dbContext.Remove(entityToDelete);
    }

    public virtual void Update(T entityToUpdate)
    {
        _dbSet.Attach(entityToUpdate);
        _dbContext.Entry(entityToUpdate).State = EntityState.Modified;
    }
}
