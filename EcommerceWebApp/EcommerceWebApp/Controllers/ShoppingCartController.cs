namespace EcommerceWebApp.Controllers
{
    public class ShoppingCartController
    {
        private readonly ShoppingCart _shoppingCart;

        public ShoppingCartController()
        {
            _shoppingCart = new ShoppingCart();
        }

        public void AddItemToCart(Item item, int quantity)
        {
            _shoppingCart.AddItem(item, quantity);
        }

        public void RemoveItemFromCart(int productId, int? Quantity)
        {
            _shoppingCart.RemoveItem(productId, Quantity);
        }

        public void ViewCart()
        {
            // Logic to view cart
        }

        public void CheckoutCart()
        {
            // Logic to checkout
        }
    }
}
