using Dotnet9.Models;
using Dotnet9.Repository.Irepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet9.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerMallsController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public CustomerMallsController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpGet]
        public async Task<ActionResult> GetCustomerMalls()
        {
            return Ok(await _uow.CustomerMalls.GetAll("Customer,Mall"));
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetCustomerMall(int id)
        {
            var customerMall = await _uow.CustomerMalls.GetById(id);
            return customerMall is null ? NotFound() : Ok(customerMall);
        }
        [HttpPost]
        public async Task<ActionResult> CreateCustomerMall(CustomerMalls customerMall)
        {
            await _uow.CustomerMalls.Add(customerMall);
            await _uow.Save();
            return CreatedAtAction(nameof(GetCustomerMall), new { id = customerMall.CustomerId }, customerMall);
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateCustomerMall(int id, CustomerMalls customerMall)
        {
            if (id != customerMall.CustomerId)
            {
                return BadRequest();
            }
            var existingCustomerMall = await _uow.CustomerMalls.GetById(id);
            if (existingCustomerMall == null)
            {
                return NotFound();
            }
            existingCustomerMall.MallId = customerMall.MallId;
            // Update other properties as needed
            _uow.CustomerMalls.Update(existingCustomerMall);
            await _uow.Save();
            return NoContent();
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteCustomerMall(int id)
        {
            var customerMall = await _uow.CustomerMalls.GetById(id);
            if (customerMall == null)
            {
                return NotFound();
            }
            _uow.CustomerMalls.Delete(customerMall);
            await _uow.Save();
            return NoContent();
        }
    }

}