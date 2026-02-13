using Dotnet9.Data;
using Dotnet9.Models;
using Dotnet9.Repository.Irepository;

namespace Dotnet9.Repository
{
    public class MallsRepository: GenericRepository<Mall>, IMallsRepository
    {
        public MallsRepository(ApplicationDbContext db) : base(db)
        {
        }
    }
}
