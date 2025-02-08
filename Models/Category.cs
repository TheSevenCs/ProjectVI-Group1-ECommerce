using System.ComponentModel.DataAnnotations;

namespace GroceryStoreAPI.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
