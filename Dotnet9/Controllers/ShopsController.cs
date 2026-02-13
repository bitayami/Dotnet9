using Dotnet9.Models;
using Dotnet9.Repository.Irepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet9.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopsController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        public ShopsController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpGet]
        public async Task<ActionResult> GetShops()
        {
            return Ok(await _uow.Shops.GetAll());
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetShop(int id)
        {
            var shop = await _uow.Shops.GetById(id);
            return shop is null ? NotFound() : Ok(shop);
        }
        [HttpPost]
        public async Task<ActionResult> CreateShop(Shop shop)
        {
            await _uow.Shops.Add(shop);
            await _uow.Save();
            return CreatedAtAction(nameof(GetShops), new { id = shop.Id }, shop);
        }
        [HttpPut]
        public async Task<ActionResult> UpdateShop(Shop shop)
        {
            var shopFromDb = await _uow.Shops.GetById(shop.Id);
            if (shopFromDb is null)
            {
                return NotFound();
            }
            shopFromDb.Name = shop.Name;
            shopFromDb.Description = shop.Description;
            shopFromDb.MallId = shop.MallId;
            _uow.Shops.Update(shopFromDb);
            await _uow.Save();
            return NoContent();
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteShop(int id)
        {
            var shopFromDb = await _uow.Shops.GetById(id);
            if (shopFromDb is null)
            {
                return NotFound();
            }
            _uow.Shops.Delete(shopFromDb);
            await _uow.Save();
            return NoContent();
        }
    }
}
