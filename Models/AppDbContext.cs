using Microsoft.EntityFrameworkCore;

namespace AspNetCoreRestApi.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductData> ProductData { get; set; }


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)  
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ProductData View
            modelBuilder.Entity<ProductData>().ToView("view_product_data");
            modelBuilder.Entity<ProductData>().HasNoKey();

        }

    }
}