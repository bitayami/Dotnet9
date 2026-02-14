namespace Dotnet9.Repository.Irepository
{
    public interface IUnitOfWork: IDisposable
    {
        IMallsRepository Malls { get; }
        IShops Shops { get; }
        IMallOwnerRepo MallOwners { get; }
        Task<int> Save();
    }
}
