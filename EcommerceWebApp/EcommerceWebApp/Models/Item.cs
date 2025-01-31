//
namespace EcommerceWebApp.Models
{
    public class Item
    {
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public float ItemPrice { get; set; }
        public string Description { get; set; }

        public string ImagePath { get; set; }
        public Item() { }

        public Item(int itemID, string itemName, int quantity, float itemPrice, string description, string imagePath)
        {
            ItemID = itemID;
            ItemName = itemName;
            Quantity = quantity;
            ItemPrice = itemPrice;
            Description=description;
            ImagePath=imagePath;   
        }
    }
}