using Dotnet9.Models;
using Dotnet9.Repository.Irepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet9.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        public CustomerController(IUnitOfWork uow)
        {
            _uow = uow;
        }
        [HttpGet]
        public async Task<ActionResult> GetCustomers()
        {
            var customers = await _uow.Customers.GetAll("Shops");
            return Ok(customers);
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetCustomer(int id)
        {
            var customer = await _uow.Customers.GetById(id);
            return customer is null ? NotFound() : Ok(customer);
        }
        [HttpPost]
        public async Task<ActionResult> CreateCustomer(Customer customer)
        {
            await _uow.Customers.Add(customer);
            await _uow.Save();
            return CreatedAtAction(nameof(GetCustomers), new { id = customer.Id }, customer);
        }
        [HttpPut]
        public async Task<ActionResult> UpdateCustomer(Customer customer)
        {
            var customerFromDb = await _uow.Customers.GetById(customer.Id);
            if (customerFromDb is null)
            {
                return NotFound();
            }
            customerFromDb.Name = customer.Name;
            customerFromDb.PhoneNumber = customer.PhoneNumber;
            _uow.Customers.Update(customerFromDb);
            await _uow.Save();
            return NoContent();
        }
        [HttpDelete]
        public async Task<ActionResult> DeleteCustomer(int id)
        {
            var customerFromDb = await _uow.Customers.GetById(id);
            if (customerFromDb is null)
            {
                return NotFound();
            }
            _uow.Customers.Delete(customerFromDb);
            await _uow.Save();
            return NoContent();
        }
    }
}
