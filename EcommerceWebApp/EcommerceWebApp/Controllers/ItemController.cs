using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using EcommerceWebApp.Models; 

namespace EcommerceWebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly ILogger<ItemController> _logger;
        private static List<Item> _items = new List<Item>();

        public ItemController(ILogger<ItemController> logger)
        {
            _logger = logger;
        }

        [HttpPost("add")]
        public ActionResult<Item> AddItem(string name, float price, int quantity)
        {
            int newId = _items.Count + 1;
            var newItem = new Item(newId, name, quantity, price);
            _items.Add(newItem);

            _logger.LogInformation($"Added item: {newItem.ItemName} (ID: {newItem.ItemID})");
            return CreatedAtAction(nameof(GetItem), new { itemID = newItem.ItemID }, newItem);
        }

        [HttpGet("{itemID}")]
        public ActionResult<Item> GetItem(int itemID)
        {
            var item = _items.FirstOrDefault(i => i.ItemID == itemID);
            if (item == null)
            {
                _logger.LogWarning($"Item with ID {itemID} not found.");
                return NotFound();
            }
            return item;
        }

        [HttpGet]
        public ActionResult<List<Item>> GetAllItems()
        {
            return _items;
        }

        [HttpDelete("{itemID}")]
        public IActionResult DeleteItem(int itemID)
        {
            var item = _items.FirstOrDefault(i => i.ItemID == itemID);
            if (item != null)
            {
                _items.Remove(item);
                _logger.LogInformation($"Removed item with ID {itemID}");
                return NoContent();
            }
            _logger.LogWarning($"Delete failed. Item with ID {itemID} not found.");
            return NotFound();
        }
    }
}
