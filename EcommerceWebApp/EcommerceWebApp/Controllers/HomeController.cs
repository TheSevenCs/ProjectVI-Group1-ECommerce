using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using YourProject.Models;

namespace YourProject.Controllers
{
    public class HomeController : Controller
    {
        // In-memory cart: store your Item objects or item IDs
        private static List<Item> CartItems = new List<Item>();

        public IActionResult Index()
        {
            return View();
        }

        // Displays the cart page
        public IActionResult Cart()
        {
            ViewBag.CartItems = CartItems;
            return View();
        }

        // Add item to cart
        [HttpPost]
        public IActionResult AddToCartAjax(int itemID)
        {
            // 1) Create or find the item. For demonstration, a simple placeholder:
            var newItem = new Item
            {
                ItemID = itemID,
                ItemName = $"Placeholder Item {itemID}",
                ItemPrice = 5.99f,
                Quantity = 1
            };

            // 2) Add this to your in-memory cart (if you have a static List<Item> CartItems)
            CartItems.Add(newItem);

            // 3) Return a JSON response with success = true
            return Json(new
            {
                success = true,
                message = $"Item {itemID} added to cart!"
            });
        }

        
        [HttpPost]
        public IActionResult IncreaseQuantity(int itemID)
        {
            var cartItem = CartItems.FirstOrDefault(ci => ci.ItemID == itemID);
            if (cartItem != null) cartItem.Quantity++;
            return RedirectToAction("Cart");
        }

        [HttpPost]
        public IActionResult DecreaseQuantity(int itemID)
        {
            var cartItem = CartItems.FirstOrDefault(ci => ci.ItemID == itemID);
            if (cartItem != null && cartItem.Quantity > 1)
                cartItem.Quantity--;
            return RedirectToAction("Cart");
        }

        [HttpPost]
        public IActionResult RemoveItem(int itemID)
        {
            var cartItem = CartItems.FirstOrDefault(ci => ci.ItemID == itemID);
            if (cartItem != null)
                CartItems.Remove(cartItem);
            return RedirectToAction("Cart");
        }
    }
}

