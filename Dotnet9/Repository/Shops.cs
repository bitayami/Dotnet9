using Dotnet9.Data;
using Dotnet9.Models;
using Dotnet9.Repository.Irepository;

namespace Dotnet9.Repository
{
    public class Shops: GenericRepository<Shop>, IShops
    {
        public Shops(ApplicationDbContext db) : base(db)
        {
        }
    }
}
