using GroceryStoreAPI.Data;
using GroceryStoreAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace GroceryStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // PUT: api/orders/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] Order order)
        {
            if (id != order.Id)
                return BadRequest("Order ID mismatch.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingOrder = await _context.Orders.FindAsync(id);
            if (existingOrder == null)
                return NotFound();

            // Replace the existing order with the updated order
            _context.Entry(existingOrder).CurrentValues.SetValues(order);
            await _context.SaveChangesAsync();

            return NoContent(); // Successfully updated with no content in response
        }

        // PATCH: api/orders/{id}
        [HttpPatch("{id}")]
        public async Task<IActionResult> PartiallyUpdateOrder(int id, [FromBody] Order order)
        {
            if (id != order.Id)
                return BadRequest("Order ID mismatch.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingOrder = await _context.Orders.FindAsync(id);
            if (existingOrder == null)
                return NotFound();

            // Only update the fields provided in the PATCH request
            if (order.CustomerName != null) 
                existingOrder.CustomerName = order.CustomerName;
            if (order.OrderDate != default) 
                existingOrder.OrderDate = order.OrderDate;
            if (order.TotalAmount != 0) 
                existingOrder.TotalAmount = order.TotalAmount;

            await _context.SaveChangesAsync();

            return NoContent(); // Successfully updated with no content in response
        }

        // DELETE: api/orders/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
                return NotFound(); // Return 404 if the order does not exist

            _context.Orders.Remove(order); // Remove the order
            await _context.SaveChangesAsync(); // Save changes to the database

            return NoContent(); // Successfully deleted with no content in response
        }

        // GET: api/orders
        [HttpGet]
        public async Task<ActionResult<List<Order>>> GetOrders()
        {
            var orders = await _context.Orders.ToListAsync();
            return Ok(orders); // Return all orders
        }

        // GET: api/orders/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrderById(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
                return NotFound(); // If the order is not found, return 404

            return Ok(order); // Return the order by id
        }
    }
}
