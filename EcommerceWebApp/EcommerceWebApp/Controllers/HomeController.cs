using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using EcommerceWebApp.Models;
using EcommerceWebApp.Handlers;

namespace EcommerceWebApp.Controllers
{
    public class HomeController : Controller
    {
        private ShoppingCartController _shoppingCartController;
        private ItemController _itemController;
        private int _userID;

        public HomeController(DBHandler dbHandler)
        {
            _itemController = new ItemController(new LoggerFactory().CreateLogger<ItemController>(), dbHandler);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            // Check for userID in cookies
            if (!Request.Cookies.ContainsKey("userID"))
            {
                // Generate a new userID if it doesn't exist
                var newUserID = Guid.NewGuid().ToString();
                Response.Cookies.Append("userID", newUserID, new CookieOptions
                {
                    Expires = DateTime.Now.AddYears(1),
                    HttpOnly = true,
                    Secure = true
                });
                _userID = Math.Abs(newUserID.GetHashCode());
            }
            else
            {
                // Retrieve the existing userID
                var userID = Request.Cookies["userID"];
                _userID = Math.Abs(userID.GetHashCode());
            }

            var dbHandler = HttpContext.RequestServices.GetService(typeof(DBHandler)) as DBHandler;
            var existingCart = dbHandler?.LoadCart(_userID);
            var items = existingCart?.items ?? new List<Item>();
            _shoppingCartController = new ShoppingCartController(_userID, items, dbHandler);
        }
        public IActionResult Index(string[] categories)
        {
            List<Item> items;

            if (categories != null && categories.Length > 0)
            {
                // Filter items based on selected categories
                items = new List<Item>();

                foreach (var category in categories)
                {
                    var categoryItems = _itemController.GetItemsByCategory(category).Value;
                    if (categoryItems != null)
                    {
                        items.AddRange(categoryItems);
                    }
                }

                items = items.DistinctBy(item => item.ItemID).ToList();
            }
            else
            {
                // If no filter is selected, it gets all items
                items = _itemController.GetAllItems().Value as List<Item> ?? new List<Item>();
            }

            return View(items);
        }


        public IActionResult Cart()
        {
            var result = _shoppingCartController.ViewCart();

            if (result is OkObjectResult okResult && okResult.Value is List<Item> cartItems)
            {
                ViewBag.CartItems = cartItems;
                return View();
            }

            return View(new List<Item>());
        }

        [HttpPost]
        public IActionResult AddToCartAjax(int itemID)
        {
            var item = _itemController.GetItem(itemID).Value as Item;
            if (item != null)
            {
                _shoppingCartController.AddItemToCart(item);
                return Json(new { success = true, message = $"{item.ItemName} added to cart!" });
            }
            return Json(new { success = false, message = "Item not found." });
        }

        [HttpPost]
        public IActionResult IncreaseQuantity(int itemID)
        {
            try
            {
                var result = _shoppingCartController.ViewCart();
                if (result is OkObjectResult okResult && okResult.Value is List<Item> cartItems)
                {
                    var cartItem = cartItems.FirstOrDefault(ci => ci.ItemID == itemID);
                    if (cartItem != null)
                    {
                        // Increase the quantity by 1
                        cartItem.Quantity++;
                        _shoppingCartController.AddItemToCart(cartItem);

                        return RedirectToAction("Cart");
                    }
                    else
                    {
                        return NotFound("Item not found in the cart.");
                    }
                }
                else
                {
                    return BadRequest("Failed to retrieve cart items.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error increasing quantity: {ex.Message}");
            }
        }


        [HttpPost]
        public IActionResult DecreaseQuantity(int itemID)
        {
            try
            {
                var result = _shoppingCartController.ViewCart();
                if (result is OkObjectResult okResult && okResult.Value is List<Item> cartItems)
                {
                    var cartItem = cartItems.FirstOrDefault(ci => ci.ItemID == itemID);
                    if (cartItem != null)
                    {
                        // Decrease the quantity by 1
                        if (cartItem.Quantity > 1)
                        {
                            cartItem.Quantity--;
                            _shoppingCartController.AddItemToCart(cartItem);
                        }
                        else
                        {
                            // Remove the item if quantity is 1
                            _shoppingCartController.RemoveItemFromCart(itemID);
                        }

                        return RedirectToAction("Cart");
                    }
                    else
                    {
                        return NotFound("Item not found in the cart.");
                    }
                }
                else
                {
                    return BadRequest("Failed to retrieve cart items.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error decreasing quantity: {ex.Message}");
            }
        }


        [HttpPost]
        public IActionResult RemoveItem(int itemID)
        {
            _shoppingCartController.RemoveItemFromCart(itemID);
            return RedirectToAction("Cart");
        }

        [HttpPost]
        public IActionResult Checkout()
        {
            _shoppingCartController.CheckoutCart();
            return RedirectToAction("Index");
        }
    }
}
