using Microsoft.EntityFrameworkCore;

namespace ClickExpress.DataAccess
{
    public class DbSession
    {
        public static string ConnectionStrings { get; set; } = string.Empty;

        private static string BuildPooledConnectionString()
        {
            var builder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(ConnectionStrings)
            {
                MaxPoolSize = 100,
                MinPoolSize = 5,
                ConnectTimeout = 15,
                ConnectRetryCount = 2
            };
            return builder.ConnectionString;
        }

        public static void Configure(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(BuildPooledConnectionString(), sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(6),
                    errorNumbersToAdd: null);
                sqlOptions.CommandTimeout(30);
            });
        }
    }
}
