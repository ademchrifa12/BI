using System.Linq.Expressions;

namespace WideWorldImportersBI.Repositories.Interfaces;

/// <summary>
/// Generic repository interface providing standard CRUD operations
/// This interface follows the Repository Pattern for clean separation of concerns
/// </summary>
/// <typeparam name="T">The entity type</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Gets all entities
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Gets all entities with included navigation properties
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);

    /// <summary>
    /// Gets entities matching a predicate
    /// </summary>
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Gets entities matching a predicate with included navigation properties
    /// </summary>
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

    /// <summary>
    /// Gets a single entity by its primary key
    /// </summary>
    Task<T?> GetByIdAsync(int id);

    /// <summary>
    /// Gets a single entity by its primary key with included navigation properties
    /// </summary>
    Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes);

    /// <summary>
    /// Gets a single entity matching a predicate
    /// </summary>
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Gets a single entity matching a predicate with included navigation properties
    /// </summary>
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

    /// <summary>
    /// Checks if any entity matches the predicate
    /// </summary>
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Counts entities matching the predicate
    /// </summary>
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);

    /// <summary>
    /// Adds a new entity
    /// </summary>
    Task<T> AddAsync(T entity);

    /// <summary>
    /// Adds multiple entities
    /// </summary>
    Task AddRangeAsync(IEnumerable<T> entities);

    /// <summary>
    /// Updates an existing entity
    /// </summary>
    void Update(T entity);

    /// <summary>
    /// Updates multiple entities
    /// </summary>
    void UpdateRange(IEnumerable<T> entities);

    /// <summary>
    /// Deletes an entity
    /// </summary>
    void Delete(T entity);

    /// <summary>
    /// Deletes multiple entities
    /// </summary>
    void DeleteRange(IEnumerable<T> entities);

    /// <summary>
    /// Gets the queryable for complex queries
    /// </summary>
    IQueryable<T> Query();

    /// <summary>
    /// Gets the queryable with AsNoTracking for read-only operations
    /// </summary>
    IQueryable<T> QueryNoTracking();
}
