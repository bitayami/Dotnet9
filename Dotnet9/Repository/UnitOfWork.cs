using Dotnet9.Data;
using Dotnet9.Repository.Irepository;

namespace Dotnet9.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public IMallsRepository Malls { get; }
        public IShops Shops { get; }
        public IMallOwnerRepo MallOwners { get; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Malls = new MallsRepository(_db);
            Shops = new Shops(_db);
            MallOwners = new MallOwnerRepo(_db);
        }

        public void Dispose()
        {
            _db.Dispose();
        }

        public async Task<int> Save()
        {
            return await _db.SaveChangesAsync();
        }
    }
}
