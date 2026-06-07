namespace ClickExpress.Domain.Models.Base
{
    public class DashboardDTO
    {
        public UserStatsDTO Users { get; set; } = new();
        public ProductStatsDTO Products { get; set; } = new();
        public OrderStatsDTO Orders { get; set; } = new();
        public ReviewStatsDTO Reviews { get; set; } = new();
        public DriverStatsDTO Drivers { get; set; } = new();
        public List<RecentActivityDTO> RecentActivity { get; set; } = [];
    }

    public class UserStatsDTO
    {
        public int Total { get; set; }
        public int Active { get; set; }
        public int NewThisMonth { get; set; }
    }

    public class ProductStatsDTO
    {
        public int Total { get; set; }
        public int Active { get; set; }
        public int OutOfStock { get; set; }
        public int Deleted { get; set; }
        public decimal TotalValue { get; set; }
        public int Categories { get; set; }
    }

    public class OrderStatsDTO
    {
        public int Total { get; set; }
        public int Pending { get; set; }
        public int Approved { get; set; }
        public int Cancelled { get; set; }
        public int NewToday { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class ReviewStatsDTO
    {
        public int Total { get; set; }
        public int Approved { get; set; }
        public int Pending { get; set; }
        public double AverageRating { get; set; }
    }

    public class DriverStatsDTO
    {
        public int Total { get; set; }
        public int Available { get; set; }
        public int OnRoute { get; set; }
    }

    public class RecentActivityDTO
    {
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
