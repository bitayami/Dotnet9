using Dotnet9.Data;
using Dotnet9.Repository.Irepository;
using Microsoft.EntityFrameworkCore;

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
        

        public async Task<T> GetById(int id)
        =>
            await _dbSet.FindAsync(id);
        

        public void Update(T entity)
        =>
            _db.Update(entity);
        
    }
}
