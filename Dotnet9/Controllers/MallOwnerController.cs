using Dotnet9.Data;
using Dotnet9.Models;
using Dotnet9.Repository.Irepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dotnet9.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MallOwnerController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly ApplicationDbContext _db;

        public MallOwnerController(IUnitOfWork uow, ApplicationDbContext db)
        {
            _uow = uow;
            _db = db;
        }
        [HttpGet]  
        public async Task<ActionResult> GetMallOwners()
        {
            //var mallOwners = await _uow.MallOwners.GetAll();
            var mallOwners = await _uow.MallOwners.GetAll(includeProperties: "Mall");
            //var mallOwners = await _db.MallOwners.Include(m => m.Mall).ToListAsync();
            return Ok(mallOwners);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetMallOwner(int id)
        {
            //var mallOwner = await _uow.MallOwners.GetById(id);
            var mallOwner = await _uow.MallOwners.GetById(i => i.Id == id, "Mall");
            return Ok(mallOwner is null ? NotFound() : Ok(mallOwner));
        }

        [HttpPost]
        public async Task<ActionResult> CreateMallOwner(MallOwner mallOwner)
        {
            await _uow.MallOwners.Add(mallOwner);
            await _uow.Save();
            return CreatedAtAction(nameof(GetMallOwners), new { id = mallOwner.Id }, mallOwner);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateMallOwner(int id, MallOwner mallOwner)
        {
            var mallOwnerFromDb = await _uow.MallOwners.GetById(id);
            if (mallOwnerFromDb is null)
            {
                return NotFound();
            }
            mallOwnerFromDb.Name = mallOwner.Name;
            mallOwnerFromDb.ContactInfo = mallOwner.ContactInfo;
            mallOwnerFromDb.MallId = mallOwner.MallId;
            _uow.MallOwners.Update(mallOwnerFromDb);
            await _uow.Save();
            return NoContent();
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteMallOwner(int id)
        {
            var mallOwnerFromDb = await _uow.MallOwners.GetById(id);
            if (mallOwnerFromDb is null)
            {
                return NotFound();
            }
            _uow.MallOwners.Delete(mallOwnerFromDb);
            await _uow.Save();
            return NoContent();

        }
    }
}
