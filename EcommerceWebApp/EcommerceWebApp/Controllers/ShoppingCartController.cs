using EcommerceWebApp.Models;

namespace EcommerceWebApp.Controllers
{
    public class ShoppingCartController
    {
        private readonly ShoppingCart _shoppingCart;

        public ShoppingCartController(int userID)
        {
            _shoppingCart = new ShoppingCart(userID);
        }

        public void AddItemToCart(Item item, int quantity)
        {
            _shoppingCart.AddItem(item, quantity);
        }

        public void RemoveItemFromCart(int productId, int? Quantity)
        {
            _shoppingCart.RemoveItem(productId, Quantity);
        }

        public List<Item> ViewCart()
        {
            return _shoppingCart.items;
        }

        public void CheckoutCart()
        {
            _shoppingCart.ClearCart();
        }
    }
}
