using Dotnet9.Repository.Irepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet9.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MallDocumentController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public MallDocumentController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpGet]
        public async Task<ActionResult> GetMallDocuments()
        {
            var mallDocuments = await _uow.MallDocuments.GetAll();
            return Ok(mallDocuments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetMallDocument(int id)
        {
            var mallDocument = await _uow.MallDocuments.GetById(id);
            return Ok(mallDocument is null ? NotFound() : Ok(mallDocument));
        }

    }
}
