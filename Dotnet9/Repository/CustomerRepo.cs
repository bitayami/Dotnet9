using Dotnet9.Data;
using Dotnet9.Models;
using Dotnet9.Repository.Irepository;

namespace Dotnet9.Repository
{
    public class CustomerRepo : GenericRepository<Customer>, ICustomerRepo
    {
        public CustomerRepo(ApplicationDbContext db) : base(db)
        {
        }
    }
}
