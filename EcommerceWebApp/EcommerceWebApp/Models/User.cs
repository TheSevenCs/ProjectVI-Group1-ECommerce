using System;
using EcommerceWebApp.Models;

namespace EcommerceWebApp.Models
{
    public class User
    {
        public int UserId { get; set; }
        private ShoppingCart _shoppingCart;

        public User(int userId)
        {
            UserId = userId;
            _shoppingCart = new ShoppingCart(userId);
        }

        internal ShoppingCart GetCart()
        {
            return _shoppingCart;
        }

        public void ClearCart()
        {
            _shoppingCart.ClearCart();
        }
    }
}