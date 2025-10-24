using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Models;
using Grocery.Core.Data;
using Microsoft.Data.Sqlite;
using Grocery.Core.Data.Repositories;
using System.Reflection.Metadata.Ecma335;

namespace Grocery.Core.Data.Repositories
{
    public class GroceryListItemsRepository : DatabaseConnection, IGroceryListItemsRepository
    {
        public List<GroceryListItem> GetAll()
        {
            OpenConnection();
            List<GroceryListItem> items = new List<GroceryListItem>();

            using (var command = Connection.CreateCommand())
            {
                command.CommandText = "SELECT Id, GroceryListId, ProductId, Amount FROM GroceryListItems";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new GroceryListItem(
                            reader.GetInt32(0), // Id
                            reader.GetInt32(1), // GroceryListId
                            reader.GetInt32(2), // ProductId
                            reader.GetInt32(3)  // Amount
                        ));
                    }
                CloseConnection();
                }
            return items;
            }
        }
    
 

        public List<GroceryListItem> GetAllOnGroceryListId(int id)
        {
               var allItems = GetAll();
               return (from item in allItems
                    where item.GroceryListId == id
                     select item).ToList();
}

        public GroceryListItem Add(GroceryListItem item)
        {
            OpenConnection();

            using (var command = Connection.CreateCommand())
            {
                command.CommandText = @"
                            INSERT INTO GroceryListItems (GroceryListId, ProductId, Amount) 
                            VALUES (@groceryListId, @productId, @amount);
                            SELECT last_insert_rowid();";

                command.Parameters.AddWithValue("@groceryListId", item.GroceryListId);
                command.Parameters.AddWithValue("@productId", item.ProductId);
                command.Parameters.AddWithValue("@amount", item.Amount);

                int newId = Convert.ToInt32(command.ExecuteScalar());
                item.Id = newId;
                CloseConnection();

            }
                return Get(item.Id);
        }

        public GroceryListItem? Delete(GroceryListItem existingItem)
        {
            if (existingItem == null)
            {
                return null;
            }

            OpenConnection();

            using (var command = Connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM GroceryListItems WHERE Id = @id";
                command.Parameters.AddWithValue("@id", existingItem.Id);
                command.ExecuteNonQuery();
                CloseConnection();
            }
   
            return existingItem;
        }

        public GroceryListItem? Get(int id)
        {
            var allItems = GetAll();
            return (from item in allItems
                    where item.Id == id
                    select item).FirstOrDefault();
        }

        public GroceryListItem? Update(GroceryListItem item)
        {
            var existingItem = Get(item.Id);

            if (existingItem == null)
            {
                return null;
            }

            OpenConnection();

            using (var command = Connection.CreateCommand())
            {
                command.CommandText = @"
                            UPDATE GroceryListItems 
                            SET GroceryListId = @groceryListId, 
                                ProductId = @productId, 
                                Amount = @amount 
                            WHERE Id = @id";

                command.Parameters.AddWithValue("@groceryListId", item.GroceryListId);
                command.Parameters.AddWithValue("@productId", item.ProductId);
                command.Parameters.AddWithValue("@amount", item.Amount);
                command.Parameters.AddWithValue("@id", item.Id);

                command.ExecuteNonQuery();
                CloseConnection();
            }

            return Get(item.Id);
        }
    }
}

