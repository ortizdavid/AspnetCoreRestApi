using Microsoft.EntityFrameworkCore;

namespace AspNetCoreRestApi.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<ProductData> ProductData { get; set; }
        public DbSet<User> Users { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)  
        {
            // Handle Postgres Timestamp
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true); 
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ProductData -> view_product_data
            modelBuilder.Entity<ProductData>().ToView("view_product_data");
            modelBuilder.Entity<ProductData>().HasNoKey();

        }

    }
}