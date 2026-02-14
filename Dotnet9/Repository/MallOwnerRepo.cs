using Dotnet9.Data;
using Dotnet9.Models;
using Dotnet9.Repository.Irepository;

namespace Dotnet9.Repository
{
    public class MallOwnerRepo : GenericRepository<MallOwner>, IMallOwnerRepo
    {
        public MallOwnerRepo(ApplicationDbContext context) : base(context)
        {
        }
    }
}
