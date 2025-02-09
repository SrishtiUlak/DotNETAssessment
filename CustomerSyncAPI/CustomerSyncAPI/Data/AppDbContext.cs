using CustomerSyncAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerSyncAPI.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Customer> Customer { get; set; }
        public DbSet<Location> Location { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Configure MSSQL connection
            optionsBuilder.UseSqlServer("Server=ULAK\\SQLEXPRESS;Database=CustomerDB;Integrated Security=True;TrustServerCertificate=True;");
        }
    }
}
