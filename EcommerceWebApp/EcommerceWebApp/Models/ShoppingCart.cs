using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("EcommerceWebApp.Handlers")]


namespace EcommerceWebApp.Models
{
    internal class ShoppingCart
    {
        public int UserID { get; private set; }
        internal List<Item> items = new List<Item>();
        internal ShoppingCart(int userID, List<Item> items) // loading shopping carts
        {
            this.UserID = userID;
            this.items = items ?? new List<Item>();
        }

        internal ShoppingCart(int userID) // new shopping carts
        {
            this.UserID = userID;
            this.items = new List<Item>();
        }

        internal float CalculatePrice()
        {
            // Calculate price
            float price = 0.0f;
            foreach(Item item in items)
            {
                price += item.ItemPrice * item.Quantity;
            }
            return price;
        }
        internal void ClearCart()
        {
            // Clear cart
            items.Clear();
        }
        internal void AddItem(Item item, int quantity)
        {
            // Add item
            item.Quantity = quantity;
            items.Add(item);
        }
        internal void RemoveItem(int itemID, int? Quantity) // leave Quantity null to remove all of an item
        {
            Item? item = items.Find(i => i.ItemID == itemID);
            if (item != null)
            {
                // Item found
                if (Quantity == null)
                {
                    items.Remove(item);
                }
                else
                {
                    // Remove specific quantity
                    item.Quantity -= (int)Quantity;
                    if (item.Quantity <= 0)
                    {
                        items.Remove(item);
                    }
                }
            }
        }
    }
}