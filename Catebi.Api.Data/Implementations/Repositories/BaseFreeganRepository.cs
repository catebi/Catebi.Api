using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

using Catebi.Api.Data.Contracts.Repositories;

namespace Catebi.Api.Data.Implementations.Repositories;

/// <summary>
/// Base repository
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public abstract class BaseFreeganRepository<T>(FreeganContext context) : IRepository<T> where T : class
{
    #region Cstor
    protected DbSet<T> DbSet = context.Set<T>();
    protected FreeganContext Context { get; set; } = context;
    #endregion

    /// <summary>
    /// Get entities
    /// </summary>
    /// <param name="filter">Filter</param>
    /// <param name="orderBy">Sorting</param>
    /// <param name="includeProperties">Navigation properties</param>
    /// <returns>List of filtered entities</returns>
    public virtual async Task<List<T>> GetAsync
    (
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        params Expression<Func<T, object>>[] includes
    )
    {
        IQueryable<T> query = DbSet;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        if (orderBy != null)
        {
            return await orderBy(query).ToListAsync();
        }
        else
        {
            return await query.ToListAsync();
        }
    }

    /// <summary>
    /// Get entities
    /// </summary>
    /// <param name="filter">Filter</param>
    /// <param name="orderBy">Sorting</param>
    /// <param name="include">Navigation properties</param>
    /// <returns>List of result entities</returns>
    public virtual async Task<List<T>> GetAsync
    (
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object?>>? include = null
    )
    {
        IQueryable<T> query = DbSet;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (include != null)
        {
            query = include(query);
        }

        if (orderBy != null)
        {
            return await orderBy(query).ToListAsync();
        }
        else
        {
            return await query.ToListAsync();
        }
    }

    /// <summary>
    /// Get entity by Id
    /// </summary>
    /// <param name="id">Id</param>
    public virtual async Task<T?> GetByIdAsync(object id) => await DbSet.FindAsync(id);

    /// <summary>
    /// Add new entity
    /// </summary>
    /// <param name="entity">New entity</param>
    public virtual async Task<T> InsertAsync(T entity)
    {
        await DbSet.AddAsync(entity);
        return entity;
    }

    /// <summary>
    /// Add new entities
    /// </summary>
    /// <param name="entities">Entities to add</param>
    public virtual async Task InsertRangeAsync(IEnumerable<T> entities)
    {
        await DbSet.AddRangeAsync(entities);
    }

    /// <summary>
    /// Update entity
    /// </summary>
    /// <param name="entityToUpdate">Entity to update</param>
    public virtual void Update(T entityToUpdate)
    {
        DbSet.Attach(entityToUpdate);
        Context.Entry(entityToUpdate).State = EntityState.Modified;
    }


    /// <summary>
    /// Get single entity
    /// </summary>
    /// <param name="filter">Filter</param>
    public async Task<T> SingleAsync
    (
        Expression<Func<T, bool>> filter,
        params Expression<Func<T, object>>[] includes
    )
    {
        IQueryable<T> query = DbSet;

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.SingleAsync(filter);
    }

    /// <summary>
    /// Get single entity or default
    /// </summary>
    /// <param name="filter">Filter</param>
    public async Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> filter) =>
        await DbSet.SingleOrDefaultAsync(filter);

    /// <summary>
    /// Get first entity
    /// </summary>
    /// <param name="filter">Filter</param>
    /// <param name="includes">Navigation properties</param>
    public async Task<T?> FirstAsync
    (
        Expression<Func<T, bool>> filter,
        params Expression<Func<T, object>>[] includes
    )
    {
        IQueryable<T> query = DbSet;
        foreach (var include in includes)
        {
            if (include != null)
            {
                query = query.Include(include);
            }
        }

        return await query.FirstAsync(filter);
    }

    /// <summary>
    /// Get first entity or default
    /// </summary>
    /// <param name="filter">Filter</param>
    /// <param name="includes">Navigation properties</param>
    public async Task<T?> FirstOrDefaultAsync
    (
        Expression<Func<T, bool>> filter,
        params Expression<Func<T, object>>[] includes
    )
    {
        IQueryable<T> query = DbSet;
        foreach (var include in includes)
        {
            if (include != null)
            {
                query = query.Include(include);
            }
        }
        return await query.FirstOrDefaultAsync(filter);
    }

    /// <summary>
    /// Get first entity or default
    /// </summary>
    /// <param name="filter">Filter</param>
    /// <param name="include">Navigation properties</param>
    public async Task<T?> FirstOrDefaultAsync
    (
        Expression<Func<T, bool>> filter, Func<IQueryable<T>,
        IIncludableQueryable<T, object>> include,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null
    )
    {
        IQueryable<T> query = DbSet;
        if (include != null)
        {
            query = include(query);
        }
        if (orderBy != null)
        {
            return await orderBy(query).FirstOrDefaultAsync(filter);
        }
        else
        {
            return await query.FirstOrDefaultAsync(filter);
        }
    }

    /// <summary>
    /// Remove collection
    /// </summary>
    /// <param name="entities">Entity collection to remove</param>
    public void RemoveRange(IEnumerable<T> entities) => DbSet.RemoveRange(entities);

    /// <summary>
    /// Remove entity
    /// </summary>
    /// <param name="entity">Entity to remove</param>
    public void Remove(T entity) => DbSet.Remove(entity);

    /// <summary>
    /// Is there any records in the table
    /// </summary>
    public async Task<bool> AnyAsync() => await DbSet.AnyAsync();

    /// <summary>
    /// Is there any records in the table with filter
    /// </summary>
    public async Task<bool> AnyAsync(Expression<Func<T, bool>> filter) => await DbSet.AnyAsync(filter);

    /// <summary>
    /// Max entity
    /// </summary>
    public async Task<int> MaxAsync(Expression<Func<T, int>> selector) => await DbSet.MaxAsync(selector);
}
