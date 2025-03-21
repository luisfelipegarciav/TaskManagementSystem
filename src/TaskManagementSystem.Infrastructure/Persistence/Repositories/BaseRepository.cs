using System.Data;
using TaskManagementSystem.Domain;

namespace TaskManagementSystem.Infrastructure.Persistence.Repositories
{
    public abstract class BaseRepository<T> : IRepository<T> where T : class
    {
        protected readonly IDatabaseContext _context;

        protected BaseRepository(IDatabaseContext context)
        {
            _context = context;
        }

        protected IDbConnection CreateConnection() => _context.CreateConnection();

        // Implement common repository methods (e.g., GetByIdAsync, GetAllAsync) here
        // that can be reused across different database implementations
        public abstract Task<T> GetByIdAsync(int id);
        public abstract Task<List<T>> GetAllAsync();
        public abstract Task<T> AddAsync(T entity);
        public abstract Task UpdateAsync(T entity);
        public abstract Task DeleteAsync(int id);
    }
}
