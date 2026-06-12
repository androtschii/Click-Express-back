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
using ClickExpress.Domain.Entities.AuditLog;

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
        public DbSet<AuditLogData> AuditLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connStr = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(DbSession.ConnectionStrings)
            {
                MaxPoolSize = 100,
                MinPoolSize = 5,
                ConnectTimeout = 15
            }.ConnectionString;

            optionsBuilder.UseSqlServer(connStr, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(6),
                    errorNumbersToAdd: null);
                sqlOptions.CommandTimeout(30);
            });
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
            modelBuilder.Entity<AuditLogData>().ToTable("AuditLogs");

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

            // Performance indexes
            modelBuilder.Entity<ReviewData>()
                .HasIndex(r => r.UserId).HasDatabaseName("IX_Reviews_UserId");
            modelBuilder.Entity<ReviewData>()
                .HasIndex(r => r.IsApproved).HasDatabaseName("IX_Reviews_IsApproved");
            modelBuilder.Entity<ReviewData>()
                .HasIndex(r => r.CreatedAt).HasDatabaseName("IX_Reviews_CreatedAt");

            modelBuilder.Entity<OrderData>()
                .HasIndex(o => o.UserId).HasDatabaseName("IX_Orders_UserId");
            modelBuilder.Entity<OrderData>()
                .HasIndex(o => o.Status).HasDatabaseName("IX_Orders_Status");
            modelBuilder.Entity<OrderData>()
                .HasIndex(o => o.CreatedAt).HasDatabaseName("IX_Orders_CreatedAt");

            modelBuilder.Entity<NotificationData>()
                .HasIndex(n => n.UserId).HasDatabaseName("IX_Notifications_UserId");
            modelBuilder.Entity<NotificationData>()
                .HasIndex(n => new { n.UserId, n.IsRead }).HasDatabaseName("IX_Notifications_UserId_IsRead");

            modelBuilder.Entity<LeadData>()
                .HasIndex(l => l.Status).HasDatabaseName("IX_Leads_Status");
            modelBuilder.Entity<LeadData>()
                .HasIndex(l => l.CreatedAt).HasDatabaseName("IX_Leads_CreatedAt");

            modelBuilder.Entity<DocumentData>()
                .HasIndex(d => d.UploadedBy).HasDatabaseName("IX_Documents_UploadedBy");
            modelBuilder.Entity<DocumentData>()
                .HasIndex(d => d.OrderId).HasDatabaseName("IX_Documents_OrderId");

            modelBuilder.Entity<NewsArticleData>()
                .HasIndex(n => n.IsPublished).HasDatabaseName("IX_NewsArticles_IsPublished");
            modelBuilder.Entity<NewsArticleData>()
                .HasIndex(n => n.PublishedAt).HasDatabaseName("IX_NewsArticles_PublishedAt");
            modelBuilder.Entity<NewsArticleData>()
                .HasIndex(n => n.AuthorId).HasDatabaseName("IX_NewsArticles_AuthorId");

            modelBuilder.Entity<CartItemData>()
                .HasIndex(ci => ci.CartId).HasDatabaseName("IX_CartItems_CartId");
            modelBuilder.Entity<ReviewData>()
                .HasIndex(r => r.ProductId).HasDatabaseName("IX_Reviews_ProductId");
            modelBuilder.Entity<OrderStatusHistoryData>()
                .HasIndex(h => h.OrderId).HasDatabaseName("IX_OrderStatusHistories_OrderId");

            base.OnModelCreating(modelBuilder);
        }
    }
}
