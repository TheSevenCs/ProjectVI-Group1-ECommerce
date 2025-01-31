using EcommerceWebApp.Controllers;
using EcommerceWebApp.Models;
using MySql.Data.MySqlClient;

namespace EcommerceWebApp.Handlers
{
    public class DBHandler
    {
        private readonly string? _connectionString;

        public DBHandler(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Get Item by ID
        public Item? GetItemByID(int itemID)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT ItemID, ItemName, ItemPrice, Quantity FROM Items WHERE ItemID = @ItemID";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ItemID", itemID);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Item
                            {
                                ItemID = reader.GetInt32(0),
                                ItemName = reader.GetString(1),
                                ItemPrice = reader.GetFloat(2),
                                Quantity = reader.GetInt32(3)
                            };
                        }
                    }
                }
            }
            return null;
        }

        // Get Items by Category
        public List<Item> GetItemsByCategory(string itemCategory)
        {
            var items = new List<Item>();
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT ItemID, ItemName, ItemPrice, Quantity FROM Items WHERE Category = @Category";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Category", itemCategory);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(new Item
                            {
                                ItemID = reader.GetInt32(0),
                                ItemName = reader.GetString(1),
                                ItemPrice = reader.GetFloat(2),
                                Quantity = reader.GetInt32(3)
                            });
                        }
                    }
                }
            }
            return items;
        }

        // Save Shopping Cart
        internal void SaveCart(ShoppingCart cart)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string deleteQuery = "DELETE FROM CartItems WHERE UserID = @UserID";
                using (var deleteCmd = new MySqlCommand(deleteQuery, conn))
                {
                    deleteCmd.Parameters.AddWithValue("@UserID", cart.UserID);
                    deleteCmd.ExecuteNonQuery();
                }

                foreach (var item in cart.items)
                {
                    string insertQuery = "INSERT INTO CartItems (UserID, ItemID, ItemName, ItemPrice, Quantity) VALUES (@UserID, @ItemID, @ItemName, @ItemPrice, @Quantity)";
                    using (var cmd = new MySqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserID", cart.UserID);
                        cmd.Parameters.AddWithValue("@ItemID", item.ItemID);
                        cmd.Parameters.AddWithValue("@ItemName", item.ItemName);
                        cmd.Parameters.AddWithValue("@ItemPrice", item.ItemPrice);
                        cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        // Load Shopping Cart
        internal ShoppingCart LoadCart(int userID)
        {
            var items = new List<Item>();

            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT ItemID, ItemName, ItemPrice, Quantity FROM CartItems WHERE UserID = @UserID";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userID);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(new Item
                            {
                                ItemID = reader.GetInt32(0),
                                ItemName = reader.GetString(1),
                                ItemPrice = reader.GetFloat(2),
                                Quantity = reader.GetInt32(3)
                            });
                        }
                    }
                }
            }

            return new ShoppingCart(userID, items);
        }

        // Update Shopping Cart
        internal void UpdateCart(int cartID, ShoppingCart cart)
        {
            DeleteCart(cartID);
            SaveCart(cart);
        }

        // Patch Cart (Partial Updates)
        internal void PatchCart(int cartID, ShoppingCart changes)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                foreach (var item in changes.items)
                {
                    string query = "UPDATE CartItems SET Quantity = @Quantity WHERE UserID = @UserID AND ItemID = @ItemID";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserID", cartID);
                        cmd.Parameters.AddWithValue("@ItemID", item.ItemID);
                        cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        // Add Item to Cart
        public void AddItem(string itemName, float price, int quantity)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = "INSERT INTO Items (ItemName, ItemPrice, Quantity) VALUES (@ItemName, @Price, @Quantity)";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ItemName", itemName);
                    cmd.Parameters.AddWithValue("@Price", price);
                    cmd.Parameters.AddWithValue("@Quantity", quantity);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Delete Item
        public void DeleteItem(int itemID)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = "DELETE FROM Items WHERE ItemID = @ItemID";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ItemID", itemID);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Delete Cart
        public void DeleteCart(int userID)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = "DELETE FROM CartItems WHERE UserID = @UserID";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userID);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        // For testing database connection
        public bool TestConnection()
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
