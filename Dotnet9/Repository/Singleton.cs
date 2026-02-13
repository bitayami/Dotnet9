using Dotnet9.Repository.Irepository;

namespace Dotnet9.Repository
{
    public class Singleton : ISingleton
    {
        private readonly Guid _guid;
        public Singleton()
        {
            _guid = Guid.NewGuid();
        }
        public string GetGuid()
        {
            return _guid.ToString();
        }
    }
}
