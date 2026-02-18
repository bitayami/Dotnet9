using Dotnet9.Data;
using Dotnet9.Repository.Irepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Dotnet9.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        private readonly DbSet<T> _dbSet;
        public GenericRepository(ApplicationDbContext db) {
            _db = db;
            _dbSet = _db.Set<T>();
        }
        public async Task Add(T entity)
        =>
           await _dbSet.AddAsync(entity);
        

        public void Delete(T entity)
        =>
            _dbSet.Remove(entity);
        

        public async Task<IEnumerable<T>> GetAll()
        =>
            await _dbSet.ToListAsync();

        public async Task<IEnumerable<T>> GetAll(string? includeProperties = null)
        {
            IQueryable<T> query = _dbSet.AsNoTracking();

            if (includeProperties != null)
            {
                var entries = includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var includeProp in entries)
                {
                    query = query.Include(includeProp.Trim());
                }
            }
            return await query.ToListAsync();
        }


        public async Task<T> GetById(int id)
        =>
            await _dbSet.FindAsync(id);
        
        public async Task<T> GetById(Expression<Func<T, bool>> filter, string? includeProperties = null)
        {
            IQueryable<T> query = _dbSet.AsNoTracking();
            if (includeProperties != null)
            {
                var entries = includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var includeProp in entries)
                {
                    query = query.Include(includeProp.Trim());
                }
            }
            return await query.FirstOrDefaultAsync(filter);
        }
        public void Update(T entity)
        =>
            _db.Update(entity);
        
    }
}
