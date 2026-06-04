using Microsoft.EntityFrameworkCore;
using ClickExpress.Domain.Entities.User;

namespace ClickExpress.DataAccess.Context
{
    public class UserContext : DbContext
    {
        public DbSet<UserData> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            DbSession.Configure(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserData>().ToTable("Users");
            base.OnModelCreating(modelBuilder);
        }
    }
}
