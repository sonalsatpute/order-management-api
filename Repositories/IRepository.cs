using System.Linq.Expressions;

namespace OrderManagementAPI.Repositories
{
    /// <summary>
    /// Generic repository interface providing common CRUD operations
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public interface IRepository<T> where T : class
    {
        // Create operations
        Task<T> CreateAsync(T entity);
        Task<IEnumerable<T>> CreateManyAsync(IEnumerable<T> entities);

        // Read operations
        Task<T?> GetByIdAsync(object id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync();
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);

        // Update operations
        Task<T> UpdateAsync(T entity);
        Task<IEnumerable<T>> UpdateManyAsync(IEnumerable<T> entities);

        // Delete operations
        Task<bool> DeleteAsync(object id);
        Task<bool> DeleteAsync(T entity);
        Task<int> DeleteManyAsync(Expression<Func<T, bool>> predicate);

        // Advanced operations
        Task<IEnumerable<T>> GetPagedAsync(int skip, int take);
        Task<IEnumerable<T>> GetPagedAsync(int skip, int take, Expression<Func<T, bool>> predicate);
        Task<IEnumerable<TResult>> SelectAsync<TResult>(Expression<Func<T, TResult>> selector);
    }
}