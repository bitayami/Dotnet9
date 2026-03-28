using Dotnet9.Data;
using Dotnet9.Models;
using Dotnet9.Repository.Irepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Dotnet9.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MallsController : ControllerBase
    {
        //private static readonly List<Mall> Malls = new()
        //{
        //    new Mall { Id = 1, Name = "Sunshine Mall", Location = "Downtown", Floors = 5 },
        //    new Mall { Id = 2, Name = "Riverfront Mall", Location = "Riverside", Floors = 3 },
        //    new Mall { Id = 3, Name = "Mountainview Mall", Location = "Hillside", Floors = 4 }
        //};

        private readonly IUnitOfWork _uow;
        private readonly ILogger<MallsController> _logger;

        public MallsController(IUnitOfWork uow, ILogger<MallsController> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult> GetMalls()
        {
            //return Ok(await _context.Malls.ToListAsync());
            _logger.LogInformation("Fetching all malls at {Time}",DateTime.Now);
            var malls = await _uow.Malls.GetAll("MallOwner,Shops,Customers");
            _logger.LogInformation("Retrived all {Count} malls", malls.Count());
            return Ok(malls);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Mall>> GetMall(int id)
        {
            //var mall = _context.Malls.FirstOrDefault(m => m.Id == id);
            _logger.LogDebug("GetMallbyId = {@id}",id);
            var mall = await _uow.Malls.GetById(id);

            if (mall == null)
            {
                _logger.LogError("Mall with id = {id} is not in DB", id);
                return NotFound();
            }
            _logger.LogInformation("Successfully retrieved mall name = {name}",mall.Name);
            return Ok(mall);
            //var mall = await _context.Malls.FindAsync(id);
            //return mall is null ? NotFound() : Ok(mall);
            //var mall = await _uow.Malls.GetById(id);
            //return mall is null ? NotFound() : Ok(mall);
        }
        [HttpPost]
        public async Task<ActionResult<Mall>> CreateMall(Mall mall)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest();
            //}
            //Console.WriteLine($"Entity state start: {_context.Entry(mall).State}");
            //_context.Malls.Add(mall);
            //await _context.SaveChangesAsync();
            //return Ok("Mall created succesfully");

            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["Operation"] = "CreateMall",
                ["MallName"] = mall.Name,
                ["RequestedBy"] = HttpContext.User.Identity?.Name ?? "anonymous"
            });

            await _uow.Malls.Add(mall);
            await _uow.Save();
            return Ok("Mall created succesfully");
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateMall(int id, Mall updatedMall)
        {
            //var mall = await _context.Malls.FindAsync(id);
            //if (mall == null)
            //{
            //    return NotFound();
            //}
            //mall.Name = updatedMall.Name;
            //mall.Location = updatedMall.Location;
            //mall.Floors = updatedMall.Floors;
            //await _context.SaveChangesAsync();
            //return NoContent();

            var mall = await _uow.Malls.GetById(id);
            if (mall == null)
            {
                return NotFound();
            }
            mall.Name = updatedMall.Name;
            mall.Location = updatedMall.Location;
            mall.Floors = updatedMall.Floors;
            _uow.Malls.Update(mall);
            await _uow.Save();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteMall(int id)
        {
            //var mall = await _context.Malls.FirstOrDefaultAsync(m => m.Id == id);
            //if (mall == null)
            //{
            //    return NotFound();
            //}
            //_context.Malls.Remove(mall);
            //await _context.SaveChangesAsync();
            //return NoContent();
            _logger.LogWarning("DeleteMallbyId = {@id}", id);

            var mall = await _uow.Malls.GetById(id);
            if (mall == null)
            {
                _logger.LogError("Mall with id = {id} is not in DB", id);
                return NotFound();
            }
            _uow.Malls.Delete(mall);
            await _uow.Save();
            _logger.LogCritical("AUDIT: Mall id = {id} deleted by user = {user} at {time}",
                id,HttpContext.User.Identity?.Name ?? "anonymous",DateTime.Now);
            return NoContent();
        }
    }
}
