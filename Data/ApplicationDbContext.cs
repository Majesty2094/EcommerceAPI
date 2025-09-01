using Microsoft.EntityFrameworkCore;
using MajesticEcommerceAPI.Models;

namespace MajesticEcommerceAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        
        public DbSet<Product> Products { get; set; }    }
}
