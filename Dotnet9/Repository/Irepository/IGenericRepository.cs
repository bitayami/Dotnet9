using System.Linq.Expressions;

namespace Dotnet9.Repository.Irepository
{
    public interface IGenericRepository<T> where T : class 
    {
        Task<IEnumerable<T>> GetAll(string? includeProperties = null);
        //Task<IEnumerable<T>> GetAll();
        Task<T> GetById(int id);
        Task<T> GetById(Expression<Func<T, bool>> filter, string? includeProperties = null);
        Task Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
