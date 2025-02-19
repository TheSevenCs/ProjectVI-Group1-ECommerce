using EcommerceWebApp.Controllers;
using EcommerceWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EcommerceWebApp.Handlers
{
    public class DBHandler
    {
        private readonly string? _connectionString;

        public DBHandler(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public Item? GetItemByID(int itemID)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT item_id, item_name, item_price, item_quantity, description FROM ITEM WHERE item_id = @ItemID";

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
                                Quantity = reader.GetInt32("item_quantity"),
                                Description = reader.GetString("description")
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
                string query = "SELECT item_id, item_name, item_price, item_quantity, description FROM ITEM";

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
                                Quantity = reader.GetInt32("item_quantity"),
                                Description = reader.GetString("description")
                            });
                        }
                    }
                }
            }
            return items;
        }

        public List<Item> GetItemsByCategory(string itemCategory)
        {
            var items = new List<Item>();
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT item_id, item_name, item_price, item_quantity, description FROM ITEM WHERE item_category = @Category";

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
                                Quantity = reader.GetInt32("item_quantity"),
                                Description = reader.GetString("description")
                            });
                        }
                    }
                }
            }
            return items;
        }

        public User GetUserById(int userId)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                var query = "SELECT * FROM Users WHERE user_id = @UserId";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.Add(new MySqlParameter("@UserId", userId));
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var user = new User((int)reader["user_id"]);
                            return user;
                        }
                        return null;
                    }
                }
            }
        }

        public void UpdateUser(int userId, User updatedUser)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                var query = "UPDATE Users SET name = @Name, email = @Email WHERE user_id = @UserId";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.Add(new MySqlParameter("@UserId", userId));
                    cmd.Parameters.Add(new MySqlParameter("@Name", updatedUser.Name));
                    cmd.Parameters.Add(new MySqlParameter("@Email", updatedUser.Email));

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void PatchUser(int userId, User updatedUser)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                var query = "UPDATE Users SET ";

                var setClause = new List<string>();
                var parameters = new List<MySqlParameter>();

                if (!string.IsNullOrEmpty(updatedUser.Name))
                {
                    setClause.Add("name = @Name");
                    parameters.Add(new MySqlParameter("@Name", updatedUser.Name));
                }

                if (!string.IsNullOrEmpty(updatedUser.Email))
                {
                    setClause.Add("email = @Email");
                    parameters.Add(new MySqlParameter("@Email", updatedUser.Email));
                }

                if (setClause.Count == 0)
                {
                    throw new ArgumentException("No fields to update.");
                }

                query += string.Join(", ", setClause);
                query += " WHERE user_id = @UserId";
                parameters.Add(new MySqlParameter("@UserId", userId));

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddRange(parameters.ToArray());
                    cmd.ExecuteNonQuery();
                }
            }
        }

        internal void SaveCart(ShoppingCart cart)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                string newCartId = Guid.NewGuid().ToString();

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

                string deleteQuery = "DELETE FROM ITEM_CART WHERE cart_id = @CartID";
                using (var deleteCmd = new MySqlCommand(deleteQuery, conn))
                {
                    deleteCmd.Parameters.AddWithValue("@CartID", newCartId);
                    deleteCmd.ExecuteNonQuery();
                }

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
                    SELECT ic.item_id, i.item_name, i.item_price, ic.quantity, i.description 
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
                                Quantity = reader.GetInt32("quantity"),
                                Description = reader.GetString("description")
                            });
                        }
                    }
                }
            }

            return new ShoppingCart(userID, items);
        }

   
        internal void UpdateCart(int cartID, ShoppingCart updatedcart)
        {
            DeleteCart(cartID);
            SaveCart(updatedcart);
        }


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
        public void PatchItem(int itemID, Item updatedItem)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                string query = "UPDATE ITEM SET ";
                List<MySqlParameter> parameters = new List<MySqlParameter>();

                if (!string.IsNullOrEmpty(updatedItem.ItemName))
                {
                    query += "item_name = @ItemName, ";
                    parameters.Add(new MySqlParameter("@ItemName", updatedItem.ItemName));
                }

                if (updatedItem.ItemPrice != 0)
                {
                    query += "item_price = @ItemPrice, ";
                    parameters.Add(new MySqlParameter("@ItemPrice", updatedItem.ItemPrice));
                }

                if (updatedItem.Quantity != 0)
                {
                    query += "item_quantity = @ItemQuantity, ";
                    parameters.Add(new MySqlParameter("@ItemQuantity", updatedItem.Quantity));
                }

                if (!string.IsNullOrEmpty(updatedItem.Description))
                {
                    query += "description = @Description, ";
                    parameters.Add(new MySqlParameter("@Description", updatedItem.Description));
                }

                query = query.TrimEnd(',', ' ');
                query += " WHERE item_id = @ItemID";
                parameters.Add(new MySqlParameter("@ItemID", itemID));

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddRange(parameters.ToArray());
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateItem(int itemID, Item updatedItem)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                string query = "UPDATE ITEM SET item_name = @ItemName, item_price = @ItemPrice, item_quantity = @ItemQuantity, description = @Description WHERE item_id = @ItemID";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ItemID", itemID);
                    cmd.Parameters.AddWithValue("@ItemName", updatedItem.ItemName);
                    cmd.Parameters.AddWithValue("@ItemPrice", updatedItem.ItemPrice);
                    cmd.Parameters.AddWithValue("@ItemQuantity", updatedItem.Quantity);
                    cmd.Parameters.AddWithValue("@Description", updatedItem.Description);
                    cmd.ExecuteNonQuery();
                }
            }
        }

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
