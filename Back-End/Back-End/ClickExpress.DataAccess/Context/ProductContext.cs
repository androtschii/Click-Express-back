using Microsoft.EntityFrameworkCore;
using ClickExpress.Domain.Entities.Product;

namespace ClickExpress.DataAccess.Context
{
    public class ProductContext : DbContext
    {
        public DbSet<ProductData> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            DbSession.Configure(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductData>().ToTable("Products");
            base.OnModelCreating(modelBuilder);
        }
    }
}
