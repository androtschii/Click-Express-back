using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClickExpress.DataAccess.Context;
using ClickExpress.Domain.Models.Base;

namespace ClickExpress.Api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class DashboardController : ControllerBase
    {
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(ILogger<DashboardController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetStats()
        {
            var dto = new DashboardDTO();

            using (var db = new UserContext())
            {
                var now = DateTime.UtcNow;
                var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

                dto.Users = new UserStatsDTO
                {
                    Total = db.Users.AsNoTracking().Count(),
                    Active = db.Users.AsNoTracking().Count(u => u.IsActive),
                    NewThisMonth = db.Users.AsNoTracking().Count(u => u.CreatedAt >= monthStart),
                };
            }

            using (var db = new ProductContext())
            {
                var products = db.Products.AsNoTracking().ToList();
                dto.Products = new ProductStatsDTO
                {
                    Total = products.Count(p => !p.IsDeleted),
                    Active = products.Count(p => p.IsActive && !p.IsDeleted),
                    OutOfStock = products.Count(p => p.Stock == 0 && p.IsActive && !p.IsDeleted),
                    Deleted = products.Count(p => p.IsDeleted),
                    TotalValue = products.Where(p => p.IsActive && !p.IsDeleted).Sum(p => p.Price * p.Stock),
                    Categories = products.Where(p => !p.IsDeleted).Select(p => p.Category).Distinct().Count(),
                };
            }

            using (var db = new OrderContext())
            {
                var today = DateTime.UtcNow.Date;
                var orders = db.Orders.AsNoTracking().ToList();

                dto.Orders = new OrderStatsDTO
                {
                    Total = orders.Count,
                    Pending = orders.Count(o => o.Status == "Pending"),
                    Approved = orders.Count(o => o.Status == "Approved"),
                    Cancelled = orders.Count(o => o.Status == "Cancelled"),
                    NewToday = orders.Count(o => o.CreatedAt.Date == today),
                    TotalRevenue = orders
                        .Where(o => o.Status == "Approved")
                        .Sum(o => o.TotalPrice),
                };

                var reviews = db.Reviews.AsNoTracking().ToList();
                dto.Reviews = new ReviewStatsDTO
                {
                    Total = reviews.Count,
                    Approved = reviews.Count(r => r.IsApproved),
                    Pending = reviews.Count(r => !r.IsApproved),
                    AverageRating = reviews.Any() ? Math.Round(reviews.Average(r => r.Rating), 2) : 0,
                };

                var drivers = db.Drivers.AsNoTracking().Where(d => !d.IsDeleted).ToList();
                dto.Drivers = new DriverStatsDTO
                {
                    Total = drivers.Count,
                    Available = drivers.Count(d => d.Status == "Available"),
                    OnRoute = drivers.Count(d => d.Status == "OnRoute"),
                };

                dto.RecentActivity = db.AuditLogs.AsNoTracking()
                    .OrderByDescending(a => a.Timestamp)
                    .Take(15)
                    .Select(a => new RecentActivityDTO
                    {
                        Type = a.EntityType,
                        Description = $"{a.Action} #{a.EntityId}" + (a.Details != null ? $" — {a.Details}" : ""),
                        Username = a.Username,
                        Timestamp = a.Timestamp,
                    })
                    .ToList();
            }

            _logger.LogInformation("Dashboard stats requested");
            return Ok(dto);
        }

        [HttpGet("revenue/monthly")]
        public IActionResult GetMonthlyRevenue([FromQuery] int months = 6)
        {
            if (months < 1 || months > 24) months = 6;

            using var db = new OrderContext();
            var cutoff = DateTime.UtcNow.AddMonths(-months);

            var data = db.Orders.AsNoTracking()
                .Where(o => o.Status == "Approved" && o.CreatedAt >= cutoff)
                .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Revenue = g.Sum(o => o.TotalPrice),
                    Count = g.Count(),
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToList();

            return Ok(data);
        }
    }
}
