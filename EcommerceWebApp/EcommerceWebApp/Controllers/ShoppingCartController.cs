using EcommerceWebApp.Handlers;
using EcommerceWebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceWebApp.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly ShoppingCart _shoppingCart;
        private readonly int _userID;
        private readonly DBHandler _dbHandler;

        public ShoppingCartController(int userID, DBHandler dbHandler)
        {
            _userID = userID;
            _dbHandler = dbHandler;

            var existingCart = dbHandler.LoadCart(userID);
            if (existingCart.items.Count == 0)
            {
                _shoppingCart = new ShoppingCart(userID);
                _dbHandler.SaveCart(_shoppingCart);
            }
            else
            {
                _shoppingCart = existingCart;
            }
        }

        public ShoppingCartController(int userID, List<Item> items, DBHandler dbHandler)
        {
            _userID = userID;
            _dbHandler = dbHandler;

            if (items.Count == 0)
            {
                    _shoppingCart = new ShoppingCart(userID);
                _dbHandler.SaveCart(_shoppingCart);
            }
            else
            {
                _shoppingCart = new ShoppingCart(userID, items);
            }
        }
            
        [HttpPost("add")]
        public IActionResult AddItemToCart(Item item)
        {
            try
            {
                _shoppingCart.AddItem(item, item.Quantity);
                _dbHandler.SaveCart(_shoppingCart);
                return Ok("Item added successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to add item: {ex.Message}");
            }
        }

        [HttpDelete("remove/{productId}")]
        public IActionResult RemoveItemFromCart(int productId, int? quantity = null)
        {
            try
            {
                _shoppingCart.RemoveItem(productId, quantity);
                _dbHandler.SaveCart(_shoppingCart);
                return Ok("Item removed successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to remove item: {ex.Message}");
            }
        }

        [HttpGet("view")]
        public IActionResult ViewCart()
        {
            return Ok(_shoppingCart.items);
        }

        [HttpPost("checkout")]
        public IActionResult CheckoutCart()
        {
            try
            {
                _shoppingCart.ClearCart();
                _dbHandler.DeleteCart(_userID);
                return Ok("Checkout successful.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Checkout failed: {ex.Message}");
            }
        }
    }
}
