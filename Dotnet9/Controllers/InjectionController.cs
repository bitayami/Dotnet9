using Dotnet9.Repository.Irepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Dotnet9.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InjectionController : ControllerBase
    {
        private readonly ITransient _transient1;
        private readonly ITransient _transient2;
        private readonly IScoped _scoped1;
        private readonly IScoped _scoped2;
        private readonly ISingleton _singleton1;
        private readonly ISingleton _singleton2;


        public InjectionController(ITransient transient1, ITransient transient2, IScoped scoped1, IScoped scoped2, ISingleton singleton1, ISingleton singleton2)
        {
            _transient1 = transient1;
            _transient2 = transient2;
            _scoped1 = scoped1;
            _scoped2 = scoped2;
            _singleton1 = singleton1;
            _singleton2 = singleton2;
        }
        [HttpGet]
        public IActionResult Get()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Transient 1: {_transient1.GetGuid()}\n");
            sb.AppendLine($"Transient 2: {_transient2.GetGuid()}\n\n");
            sb.AppendLine($"Scoped 1: {_scoped1.GetGuid()}\n");
            sb.AppendLine($"Scoped 2: {_scoped2.GetGuid()}\n\n");
            sb.AppendLine($"Singleton 1: {_singleton1.GetGuid()}\n");
            sb.AppendLine($"Singleton 2: {_singleton2.GetGuid()}\n\n");
            Console.WriteLine(sb.ToString());

            return Ok(sb.ToString());
        }
    }
}
