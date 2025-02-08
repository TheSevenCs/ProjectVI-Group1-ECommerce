using Microsoft.EntityFrameworkCore;
using GroceryStoreAPI.Models;
using System.Linq;

namespace GroceryStoreAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set;}
        public DbSet<Category> Categories { get; set;}
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
    }
}
