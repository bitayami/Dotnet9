using Dotnet9.Data;
using Dotnet9.Models;
using Dotnet9.Repository.Irepository;

namespace Dotnet9.Repository
{
    public class CustomerMallsRepo : GenericRepository<CustomerMalls>, ICustomerMallsRepo
    {
        public CustomerMallsRepo(ApplicationDbContext db) : base(db)
        {
        }
    }
}
