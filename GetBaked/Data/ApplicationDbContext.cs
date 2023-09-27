using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GetBaked.Models;

namespace GetBaked.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        // DbSets for each table that can do CRUD
        public DbSet<GetBaked.Models.Product> Products { get; set; } = default!;
        public DbSet<Category> Categories { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }    
        public DbSet<OrderDetail> OrderDetails { get; set; }

    }
}