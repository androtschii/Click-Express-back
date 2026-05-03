using Microsoft.EntityFrameworkCore;
using back_end.Domain;

namespace back_end.DAL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<NewsArticle> NewsArticles { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }
        public DbSet<Lead> Leads { get; set; }
        public DbSet<SavedLoad> SavedLoads { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Product)
                .WithMany()
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NewsArticle>()
                .HasOne(n => n.Author)
                .WithMany()
                .HasForeignKey(n => n.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SavedLoad>()
                .HasOne(sl => sl.User)
                .WithMany()
                .HasForeignKey(sl => sl.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SavedLoad>()
                .HasOne(sl => sl.Product)
                .WithMany()
                .HasForeignKey(sl => sl.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SavedLoad>()
                .HasIndex(sl => new { sl.UserId, sl.ProductId })
                .IsUnique();

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Colorado Springs → Tampa", Description = "Flatbed / Oversized Containers", Price = 4100m, ImageUrl = "/images/real9.jpg", Category = "Full Load", Stock = 1, IsActive = true, CreatedAt = new DateTime(2026, 1, 1) },
                new Product { Id = 2, Name = "Madera → Fort Collins", Description = "Flatbed / HVAC Units", Price = 5120m, ImageUrl = "/images/real2.jpg", Category = "Full Load", Stock = 1, IsActive = true, CreatedAt = new DateTime(2026, 1, 1) },
                new Product { Id = 3, Name = "Deer Park → Jackson", Description = "Flatbed / Steel Beams", Price = 2100m, ImageUrl = "/images/real3.jpg", Category = "Full Load", Stock = 1, IsActive = true, CreatedAt = new DateTime(2026, 1, 1) },
                new Product { Id = 4, Name = "Salt Lake City → Houston", Description = "Stepdeck / Heavy Machinery", Price = 5500m, ImageUrl = "/images/real4.jpg", Category = "Partial", Stock = 1, IsActive = true, CreatedAt = new DateTime(2026, 1, 1) },
                new Product { Id = 5, Name = "Key Largo → Lake Ozark", Description = "Flatbed / Equipment", Price = 4000m, ImageUrl = "/images/real5.jpg", Category = "Full Load", Stock = 1, IsActive = true, CreatedAt = new DateTime(2026, 1, 1) },
                new Product { Id = 6, Name = "Anniston → Apache Junction", Description = "Military / Multi-Stop", Price = 19499m, ImageUrl = "/images/real6.jpg", Category = "Military Load", Stock = 1, IsActive = true, CreatedAt = new DateTime(2026, 1, 1) },
                new Product { Id = 7, Name = "Connellsville → Snyder", Description = "Flatbed / Heavy Equipment", Price = 3100m, ImageUrl = "/images/real7.jpg", Category = "Full Load", Stock = 1, IsActive = true, CreatedAt = new DateTime(2026, 1, 1) },
                new Product { Id = 8, Name = "Las Vegas → Carnesville", Description = "Flatbed / Construction Equipment", Price = 6350m, ImageUrl = "/images/real8.jpg", Category = "Partial", Stock = 1, IsActive = true, CreatedAt = new DateTime(2026, 1, 1) },
                new Product { Id = 9, Name = "Houston → Jackson", Description = "Flatbed / Multi-Stop Steel", Price = 14900m, ImageUrl = "/images/real1.jpg", Category = "Full Load", Stock = 1, IsActive = true, CreatedAt = new DateTime(2026, 1, 1) },
                new Product { Id = 10, Name = "Phoenix → Memphis", Description = "Flatbed / Heavy Equipment", Price = 6800m, ImageUrl = "/images/real10.jpg", Category = "Full Load", Stock = 1, IsActive = true, CreatedAt = new DateTime(2026, 1, 1) }
            );
        }
    }
}
