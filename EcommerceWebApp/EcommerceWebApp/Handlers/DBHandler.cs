using EcommerceWebApp.Controllers;
using EcommerceWebApp.Models;
using Microsoft.AspNetCore.Mvc;
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
                string query = "SELECT item_id, item_name, item_price, item_quantity FROM ITEM WHERE item_id = @ItemID";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ItemID", itemID);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Item
                            {
                                ItemID = reader.GetInt32("item_id"),
                                ItemName = reader.GetString("item_name"),
                                ItemPrice = reader.GetFloat("item_price"),
                                Quantity = reader.GetInt32("item_quantity")
                            };
                        }
                    }
                }
            }
            return null;
        }

        [HttpGet]
        public ActionResult<List<Item>> GetAllItems()
        {
            var items = new List<Item>();
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT item_id, item_name, item_price, item_quantity FROM ITEM";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(new Item
                            {
                                ItemID = reader.GetInt32("item_id"),
                                ItemName = reader.GetString("item_name"),
                                ItemPrice = reader.GetFloat("item_price"),
                                Quantity = reader.GetInt32("item_quantity")
                            });
                        }
                    }
                }
            }
            return items;
        }

        // Get Items by Category
        public List<Item> GetItemsByCategory(string itemCategory)
        {
            var items = new List<Item>();
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT item_id, item_name, item_price, item_quantity FROM ITEM WHERE item_category = @Category";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Category", itemCategory);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(new Item
                            {
                                ItemID = reader.GetInt32("item_id"),
                                ItemName = reader.GetString("item_name"),
                                ItemPrice = reader.GetFloat("item_price"),
                                Quantity = reader.GetInt32("item_quantity")
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

                string newCartId = Guid.NewGuid().ToString();

                // Ensure cart exists
                string cartExistQuery = "SELECT cart_id FROM CART WHERE user_id = @UserID";
                using (var cartCheckCmd = new MySqlCommand(cartExistQuery, conn))
                {
                    cartCheckCmd.Parameters.AddWithValue("@UserID", cart.UserID.ToString());
                    var existingCartId = cartCheckCmd.ExecuteScalar();

                    if (existingCartId == null)
                    {
                        string insertCartQuery = "INSERT INTO CART (cart_id, user_id) VALUES (@CartID, @UserID)";
                        using (var insertCartCmd = new MySqlCommand(insertCartQuery, conn))
                        {
                            insertCartCmd.Parameters.AddWithValue("@CartID", newCartId);
                            insertCartCmd.Parameters.AddWithValue("@UserID", cart.UserID.ToString());
                            insertCartCmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        newCartId = existingCartId.ToString();
                    }
                }

                // Remove old cart items
                string deleteQuery = "DELETE FROM ITEM_CART WHERE cart_id = @CartID";
                using (var deleteCmd = new MySqlCommand(deleteQuery, conn))
                {
                    deleteCmd.Parameters.AddWithValue("@CartID", newCartId);
                    deleteCmd.ExecuteNonQuery();
                }

                // Insert items into ITEM_CART
                foreach (var item in cart.items)
                {
                    string insertQuery = "INSERT INTO ITEM_CART (cart_id, item_id, quantity) VALUES (@CartID, @ItemID, @Quantity)";
                    using (var cmd = new MySqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@CartID", newCartId);
                        cmd.Parameters.AddWithValue("@ItemID", item.ItemID);
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

                string getCartIdQuery = "SELECT cart_id FROM CART WHERE user_id = @UserID";
                string? cartId = null;

                using (var getCartIdCmd = new MySqlCommand(getCartIdQuery, conn))
                {
                    getCartIdCmd.Parameters.AddWithValue("@UserID", userID.ToString());
                    cartId = getCartIdCmd.ExecuteScalar()?.ToString();
                }

                if (string.IsNullOrEmpty(cartId))
                {
                    return new ShoppingCart(userID, items);
                }

                string query = @"
                    SELECT ic.item_id, i.item_name, i.item_price, ic.quantity 
                    FROM ITEM_CART ic 
                    JOIN ITEM i ON ic.item_id = i.item_id 
                    WHERE ic.cart_id = @CartID";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CartID", cartId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(new Item
                            {
                                ItemID = reader.GetInt32("item_id"),
                                ItemName = reader.GetString("item_name"),
                                ItemPrice = reader.GetFloat("item_price"),
                                Quantity = reader.GetInt32("quantity")
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

        // Patch Cart
        internal void PatchCart(int cartID, ShoppingCart changes)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                foreach (var item in changes.items)
                {
                    string query = "UPDATE ITEM_CART SET quantity = @Quantity WHERE cart_id = @CartID AND item_id = @ItemID";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CartID", cartID.ToString());
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
                string query = "INSERT INTO ITEM (item_name, item_price, item_quantity) VALUES (@ItemName, @Price, @Quantity)";

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
                string query = "DELETE FROM ITEM WHERE item_id = @ItemID";

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

                string getCartIdQuery = "SELECT cart_id FROM CART WHERE user_id = @UserID";
                string? cartId = null;

                using (var getCartIdCmd = new MySqlCommand(getCartIdQuery, conn))
                {
                    getCartIdCmd.Parameters.AddWithValue("@UserID", userID.ToString());
                    cartId = getCartIdCmd.ExecuteScalar()?.ToString();
                }

                if (string.IsNullOrEmpty(cartId))
                {
                    return;
                }

                string deleteItemsQuery = "DELETE FROM ITEM_CART WHERE cart_id = @CartID";
                using (var deleteItemsCmd = new MySqlCommand(deleteItemsQuery, conn))
                {
                    deleteItemsCmd.Parameters.AddWithValue("@CartID", cartId);
                    deleteItemsCmd.ExecuteNonQuery();
                }

                string deleteCartQuery = "DELETE FROM CART WHERE cart_id = @CartID";
                using (var deleteCartCmd = new MySqlCommand(deleteCartQuery, conn))
                {
                    deleteCartCmd.Parameters.AddWithValue("@CartID", cartId);
                    deleteCartCmd.ExecuteNonQuery();
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
