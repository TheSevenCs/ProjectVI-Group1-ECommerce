using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using EcommerceWebApp.Models;

namespace EcommerceWebApp.Controllers
{
    public class HomeController : Controller
    {
        private static List<Item> CartItems = new List<Item>();

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Cart()
        {
            ViewBag.CartItems = CartItems;
            return View();
        }

        [HttpPost]
        public IActionResult AddToCartAjax(int itemID)
        {
            var newItem = new Item
            {
                ItemID = itemID,
                ItemName = $"Placeholder Item {itemID}",
                ItemPrice = 5.99f,
                Quantity = 1
            };

            CartItems.Add(newItem);

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
            if (cartItem != null)
            {
                if (cartItem.Quantity > 1)
                {
                    cartItem.Quantity--;
                }
                else
                {
                    CartItems.Remove(cartItem);
                }
            }
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

        [HttpPost]
        public IActionResult Checkout()
        {
            CartItems.Clear();

            return RedirectToAction("Index");
        }
    }
}

