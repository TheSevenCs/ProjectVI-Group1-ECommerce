using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using EcommerceWebApp.Models;
using EcommerceWebApp.Handlers;
using MySql.Data.MySqlClient;

namespace EcommerceWebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly ILogger<ItemController> _logger;
        private readonly DBHandler _dbHandler;

        public ItemController(ILogger<ItemController> logger, DBHandler dbHandler)
        {
            _logger = logger;
            _dbHandler = dbHandler;
        }

        [HttpPost("add")]
        public ActionResult<Item> AddItem(string name, float price, int quantity, string description, string? imagePath = null)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                imagePath = "images/Placeholder.png";
            }

            var newItem = new Item
            {
                ItemName = name,
                ItemPrice = price,
                Quantity = quantity,
                ImagePath = imagePath,
                Description = description
            };

            _dbHandler.AddItem(name, price, quantity, imagePath, description);
            _logger.LogInformation($"Added item: {name}, Image: {imagePath}");

            return CreatedAtAction(nameof(GetItem), new { itemID = newItem.ItemID }, newItem);
        }


        [HttpGet("{itemID}")]
        public ActionResult<Item> GetItem(int itemID)
        {
            var item = _dbHandler.GetItemByID(itemID);
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
            var items = _dbHandler.GetAllItems();
            return items;
        }


        [HttpGet("category/{category}")]
        public ActionResult<List<Item>> GetItemsByCategory(string category)
        {
            var items = _dbHandler.GetItemsByCategory(category);
            if (items == null || items.Count == 0)
            {
                _logger.LogWarning($"No items found for category: {category}");
                return NotFound($"No items found for category: {category}");
            }
            return items;
        }

        [HttpPut("{itemID}")]
        public IActionResult UpdateItem(int itemID, [FromBody] Item updatedItem)
        {
            var existingItem = _dbHandler.GetItemByID(itemID);
            if (existingItem == null)
            {
                return NotFound();
            }

            _dbHandler.UpdateItem(itemID, updatedItem);
            return Ok(updatedItem);
        }

        [HttpPatch("{itemID}")]
        public IActionResult PatchItem(int itemID, [FromBody] Item updatedItem)
        {
            var existingItem = _dbHandler.GetItemByID(itemID);
            if (existingItem == null)
            {
                return NotFound();
            }

            _dbHandler.PatchItem(itemID, updatedItem);
            return Ok(updatedItem);
        }

        [HttpDelete("{itemID}")]
        public IActionResult DeleteItem(int itemID)
        {
            var item = _dbHandler.GetItemByID(itemID);
            if (item != null)
            {
                _dbHandler.DeleteItem(itemID);
                _logger.LogInformation($"Removed item with ID {itemID}");
                return NoContent();
            }
            _logger.LogWarning($"Delete failed. Item with ID {itemID} not found.");
            return NotFound();
        }

        [HttpOptions]
        public IActionResult OptionsItem()
        {
            Response.Headers.Add("Allow", "GET, POST, PUT, PATCH, DELETE, OPTIONS");
            return Ok();
        }
    }
}
