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

        public ShoppingCartController(DBHandler dbHandler)
        {
            _dbHandler = dbHandler;
            _userID = GetUserIdFromContext(); // Retrieve userID dynamically
            _shoppingCart = LoadOrCreateCart();
        }

        private int GetUserIdFromContext()
        {
            var userID = HttpContext?.Request.Cookies["userID"];
            return userID != null ? Math.Abs(userID.GetHashCode()) : 0;
        }

        private ShoppingCart LoadOrCreateCart()
        {
            var existingCart = _dbHandler.LoadCart(_userID);
            return existingCart?.items.Count > 0 ? existingCart : new ShoppingCart(_userID);
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

        [HttpPut("update")]
        public IActionResult UpdateCart([FromBody] ShoppingCart updatedCart)
        {
            try
            {
                _shoppingCart.items = updatedCart.items;
                _dbHandler.SaveCart(_shoppingCart);
                return Ok(updatedCart);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update cart: {ex.Message}");
            }
        }

        [HttpPatch("modify")]
        public IActionResult ModifyCart([FromBody] Item updatedItem)
        {
            try
            {
                _shoppingCart.AddItem(updatedItem, updatedItem.Quantity);
                _dbHandler.SaveCart(_shoppingCart);
                return Ok("Item modified in cart.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to modify cart: {ex.Message}");
            }
        }

        [HttpOptions]
        public IActionResult OptionsCart()
        {
            Response.Headers.Add("Allow", "GET, POST, PUT, PATCH, DELETE, OPTIONS");
            return Ok();
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
