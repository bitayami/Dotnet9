namespace Dotnet9.Repository.Irepository
{
    public interface IUnitOfWork: IDisposable
    {
        IMallsRepository Malls { get; }
        IShops Shops { get; }
        Task<int> Save();
    }
}
