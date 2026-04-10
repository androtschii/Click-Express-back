using Microsoft.EntityFrameworkCore;
using back_end.Domain;

namespace back_end.DAL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Экспресс-доставка", Description = "Доставка груза в течение 24 часов по всей стране", Price = 49.99m, ImageUrl = "https://placehold.co/400x400?text=Express", Category = "Доставка", Stock = 999, IsActive = true, CreatedAt = new DateTime(2026, 1, 1) },
                new Product { Id = 2, Name = "Стандартная доставка", Description = "Доставка груза в течение 3-5 рабочих дней", Price = 19.99m, ImageUrl = "https://placehold.co/400x400?text=Standard", Category = "Доставка", Stock = 999, IsActive = true, CreatedAt = new DateTime(2026, 1, 1) },
                new Product { Id = 3, Name = "Доставка негабарита", Description = "Перевозка крупногабаритных и тяжёлых грузов", Price = 199.99m, ImageUrl = "https://placehold.co/400x400?text=Heavy", Category = "Грузоперевозки", Stock = 50, IsActive = true, CreatedAt = new DateTime(2026, 1, 1) },
                new Product { Id = 4, Name = "Международная доставка", Description = "Доставка грузов в 48 штатов США", Price = 299.99m, ImageUrl = "https://placehold.co/400x400?text=International", Category = "Международная", Stock = 100, IsActive = true, CreatedAt = new DateTime(2026, 1, 1) },
                new Product { Id = 5, Name = "Хрупкий груз", Description = "Бережная перевозка хрупких и ценных грузов", Price = 79.99m, ImageUrl = "https://placehold.co/400x400?text=Fragile", Category = "Спецперевозка", Stock = 30, IsActive = true, CreatedAt = new DateTime(2026, 1, 1) },
                new Product { Id = 6, Name = "Ночная доставка", Description = "Доставка груза с ночным временным слотом", Price = 89.99m, ImageUrl = "https://placehold.co/400x400?text=Night", Category = "Доставка", Stock = 20, IsActive = false, CreatedAt = new DateTime(2026, 1, 1) }
            );
        }
    }
}
