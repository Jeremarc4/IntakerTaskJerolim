using System.Linq.Expressions;

namespace Base.Data
{

    public interface IRepository<T> where T : class
    {
        Task AddAsync(T entity);
        Task<T?> GetSingleAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<T, TResult>> expression);
        Task UpdateAsync(T entity);
        Task SaveAsync();
    }
}
