using BookStore.DAL.Data;
using BookStore.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStore.DAL.Repositories.Implementations
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly BookstoreContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(BookstoreContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public async Task RemoveAsync(int id)
        {
            T? entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }
    }
}
