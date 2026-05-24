using Microsoft.EntityFrameworkCore;
using ClickExpress.Domain.Entities.User;
using ClickExpress.Domain.Entities.Product;
using ClickExpress.Domain.Entities.Order;
using ClickExpress.Domain.Entities.Cart;
using ClickExpress.Domain.Entities.Vehicle;
using ClickExpress.Domain.Entities.Driver;
using ClickExpress.Domain.Entities.Review;
using ClickExpress.Domain.Entities.News;
using ClickExpress.Domain.Entities.Lead;
using ClickExpress.Domain.Entities.JobApplication;
using ClickExpress.Domain.Entities.Document;
using ClickExpress.Domain.Entities.SavedLoad;
using ClickExpress.Domain.Entities.Notification;

namespace ClickExpress.DataAccess.Context
{
    public class OrderContext : DbContext
    {
        public DbSet<UserData> Users { get; set; }
        public DbSet<ProductData> Products { get; set; }
        public DbSet<OrderData> Orders { get; set; }
        public DbSet<OrderItemData> OrderItems { get; set; }
        public DbSet<OrderStatusHistoryData> OrderStatusHistories { get; set; }
        public DbSet<CartData> Carts { get; set; }
        public DbSet<CartItemData> CartItems { get; set; }
        public DbSet<VehicleData> Vehicles { get; set; }
        public DbSet<DriverData> Drivers { get; set; }
        public DbSet<ReviewData> Reviews { get; set; }
        public DbSet<NewsArticleData> NewsArticles { get; set; }
        public DbSet<LeadData> Leads { get; set; }
        public DbSet<JobApplicationData> JobApplications { get; set; }
        public DbSet<DocumentData> Documents { get; set; }
        public DbSet<SavedLoadData> SavedLoads { get; set; }
        public DbSet<NotificationData> Notifications { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(DbSession.ConnectionStrings);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserData>().ToTable("Users");
            modelBuilder.Entity<ProductData>().ToTable("Products");
            modelBuilder.Entity<OrderData>().ToTable("Orders");
            modelBuilder.Entity<OrderItemData>().ToTable("OrderItems");
            modelBuilder.Entity<OrderStatusHistoryData>().ToTable("OrderStatusHistories");
            modelBuilder.Entity<CartData>().ToTable("Carts");
            modelBuilder.Entity<CartItemData>().ToTable("CartItems");
            modelBuilder.Entity<VehicleData>().ToTable("Vehicles");
            modelBuilder.Entity<DriverData>().ToTable("Drivers");
            modelBuilder.Entity<ReviewData>().ToTable("Reviews");
            modelBuilder.Entity<NewsArticleData>().ToTable("NewsArticles");
            modelBuilder.Entity<LeadData>().ToTable("Leads");
            modelBuilder.Entity<JobApplicationData>().ToTable("JobApplications");
            modelBuilder.Entity<DocumentData>().ToTable("Documents");
            modelBuilder.Entity<SavedLoadData>().ToTable("SavedLoads");
            modelBuilder.Entity<NotificationData>().ToTable("Notifications");

            modelBuilder.Entity<OrderItemData>()
                .HasOne(oi => oi.Product).WithMany().HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CartItemData>()
                .HasOne(ci => ci.Product).WithMany().HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReviewData>()
                .HasOne(r => r.Product).WithMany().HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReviewData>()
                .HasOne(r => r.User).WithMany().HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NewsArticleData>()
                .HasOne(n => n.Author).WithMany().HasForeignKey(n => n.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SavedLoadData>()
                .HasOne(sl => sl.User).WithMany().HasForeignKey(sl => sl.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SavedLoadData>()
                .HasOne(sl => sl.Product).WithMany().HasForeignKey(sl => sl.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SavedLoadData>()
                .HasIndex(sl => new { sl.UserId, sl.ProductId }).IsUnique();

            modelBuilder.Entity<DriverData>()
                .HasOne(d => d.Vehicle).WithMany().HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderData>()
                .HasOne(o => o.Vehicle).WithMany().HasForeignKey(o => o.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderData>()
                .HasOne(o => o.Driver).WithMany().HasForeignKey(o => o.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderStatusHistoryData>()
                .HasOne(h => h.Order).WithMany().HasForeignKey(h => h.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DocumentData>()
                .HasOne(d => d.Order).WithMany().HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<DocumentData>()
                .HasOne(d => d.UploadedByUser).WithMany().HasForeignKey(d => d.UploadedBy)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
