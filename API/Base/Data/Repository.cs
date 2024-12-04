using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Base.Data
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly IntakerDbContext _context;
        private readonly DbSet<T> _dbSet;
        public Repository(IntakerDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<T, TResult>> expression)
        {
            return await _dbSet.Select(expression).ToListAsync();
        }

        public async Task<T?> GetSingleAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
