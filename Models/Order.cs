using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GroceryStoreAPI.Models
{
    public class Order
    {
        internal object CustomerName;

        public int Id { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public List<Product> Products { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Pending";
        public object OrderDate { get; internal set; }
    }
}
