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

    public async Task<T> GetById(object id)
    {
        var obj = await _dbSet.FindAsync(id);

        if (obj == null)
        {
            throw new DataException($"Could not get object with id {id}");
        }

        return obj;
    }
    
    public IList<T> GetAll()
    {
        return _dbSet.ToList();
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
