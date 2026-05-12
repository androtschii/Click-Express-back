using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using back_end.DAL;

namespace back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _db;

        public AdminController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("stats")]
        public IActionResult GetStats()
        {
            var now = DateTime.UtcNow;
            var cutoff = now.AddDays(-30);

            var orders = _db.Orders.Include(o => o.Product).ToList();
            var users  = _db.Users.ToList();
            var leads  = _db.Leads.ToList();

            decimal totalRevenue = orders
                .Where(o => o.Status != "Cancelled")
                .Sum(o => o.TotalPrice ?? o.Product?.Price ?? 0m);

            decimal revenue30d = orders
                .Where(o => o.Status != "Cancelled" && o.CreatedAt >= cutoff)
                .Sum(o => o.TotalPrice ?? o.Product?.Price ?? 0m);

            var statusBreakdown = orders
                .GroupBy(o => o.Status)
                .Select(g => new { status = g.Key, count = g.Count() })
                .OrderByDescending(x => x.count)
                .ToList<object>();

            var topRoutes = orders
                .Where(o => o.Status != "Cancelled")
                .GroupBy(o => o.Product?.Name ?? "Unknown")
                .Select(g => new { route = g.Key, count = g.Count() })
                .OrderByDescending(g => g.count)
                .Take(5)
                .ToList<object>();

            var leadBreakdown = leads
                .GroupBy(l => l.Status)
                .Select(g => new { status = g.Key, count = g.Count() })
                .ToList<object>();

            int convertedLeads = leads.Count(l => l.Status == "Converted");

            return Ok(new
            {
                totalOrders   = orders.Count,
                orders30d     = orders.Count(o => o.CreatedAt >= cutoff),
                activeOrders  = orders.Count(o => o.Status != "Cancelled"),
                totalRevenue,
                revenue30d,
                totalUsers    = users.Count,
                newUsers30d   = users.Count(u => u.CreatedAt >= cutoff),
                totalLeads    = leads.Count,
                conversionRate = leads.Count > 0
                    ? Math.Round((double)convertedLeads / leads.Count * 100, 1)
                    : 0.0,
                statusBreakdown,
                topRoutes,
                leadBreakdown,
            });
        }
    }
}
