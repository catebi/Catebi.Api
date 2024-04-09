using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace Catebi.Api.Data.Contracts.Repositories;

/// <summary>
/// Base repository
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Get entities
    /// </summary>
    /// <param name="filter">Filter</param>
    /// <param name="orderBy">Sorting</param>
    /// <param name="includeProperties">Navigation properties</param>
    /// <returns>List of filtered entities with navigation properties</returns>
    Task<List<T>> GetAsync(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        params Expression<Func<T, object>>[] includes);

    /// <summary>
    /// Get entities
    /// </summary>
    /// <param name="filter">Filter</param>
    /// <param name="orderBy">Sort</param>
    /// <param name="include">Navigation properties</param>
    /// <returns>List of filtered entities with navigation properties</returns>
    Task<List<T>> GetAsync(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object?>>? include = null);

    /// <summary>
    /// Get entity by Id
    /// </summary>
    /// <param name="id">Id</param>
    Task<T?> GetByIdAsync(object id);

    /// <summary>
    /// Add new entity
    /// </summary>
    /// <param name="entity">New entity</param>
    Task<T> InsertAsync(T entity);

    /// <summary>
    /// Add new entities
    /// </summary>
    /// <param name="entities"></param>
    Task InsertRangeAsync(IEnumerable<T> entities);

    /// <summary>
    /// Update enity
    /// </summary>
    /// <param name="entityToUpdate">entity to update</param>
    void Update(T entityToUpdate);

    /// <summary>
    /// Get single entity
    /// </summary>
    /// <param name="filter">Filter</param>
    Task<T> SingleAsync(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IIncludableQueryable<T, object?>>? include = null);

    /// <summary>
    /// Get single entity or default
    /// </summary>
    /// <param name="filter">Filter</param>
    Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> filter);

    /// <summary>
    /// Get first entity or default
    /// </summary>
    /// <param name="filter">Filter</param>
    /// <param name="includes">Navigation properties</param>
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes);

    /// <summary>
    /// Get first entity
    /// </summary>
    /// <param name="filter">Filter</param>
    /// <param name="includes">Navigation properties</param>
    Task<T?> FirstAsync(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes);

    /// <summary>
    /// Get first entity or default
    /// </summary>
    /// <param name="filter">Filter</param>
    /// <param name="include">Navigation properties</param>
    Task<T?> FirstOrDefaultAsync
    (
        Expression<Func<T, bool>> filter,
        Func<IQueryable<T>, IIncludableQueryable<T, object>> include,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null
    );

    /// <summary>
    /// Remove collection
    /// </summary>
    /// <param name="entities">entity collection to remove</param>
    void RemoveRange(IEnumerable<T> entities);

    /// <summary>
    /// Remove entity
    /// </summary>
    /// <param name="entity">Entity to remove</param>
    void Remove(T entity);

    /// <summary>
    /// Is there any records in the table
    /// </summary>
    Task<bool> AnyAsync();

    /// <summary>
    /// Is there any records in the table with filter
    /// </summary>
    Task<bool> AnyAsync(Expression<Func<T, bool>> filter);

    /// <summary>
    /// Max
    /// </summary>
    Task<int> MaxAsync(Expression<Func<T, int>> selector);
}
