using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClickExpress.DataAccess.Context;

namespace ClickExpress.Api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        [HttpGet("stats")]
        public IActionResult GetStats()
        {
            using var db = new OrderContext();
            var now = DateTime.UtcNow;
            var cutoff = now.AddDays(-30);

            var orders = db.Orders.Include(o => o.Product).ToList();
            var users  = db.Users.ToList();
            var leads  = db.Leads.ToList();

            decimal totalRevenue = orders.Where(o => o.Status != "Cancelled")
                .Sum(o => o.TotalPrice ?? o.Product?.Price ?? 0m);

            decimal revenue30d = orders.Where(o => o.Status != "Cancelled" && o.CreatedAt >= cutoff)
                .Sum(o => o.TotalPrice ?? o.Product?.Price ?? 0m);

            var statusBreakdown = orders.GroupBy(o => o.Status)
                .Select(g => new { status = g.Key, count = g.Count() })
                .OrderByDescending(x => x.count).ToList<object>();

            var topRoutes = orders.Where(o => o.Status != "Cancelled")
                .GroupBy(o => o.Product?.Name ?? "Unknown")
                .Select(g => new { route = g.Key, count = g.Count() })
                .OrderByDescending(g => g.count).Take(5).ToList<object>();

            var leadBreakdown = leads.GroupBy(l => l.Status)
                .Select(g => new { status = g.Key, count = g.Count() }).ToList<object>();

            int convertedLeads = leads.Count(l => l.Status == "Converted");

            return Ok(new
            {
                totalOrders   = orders.Count,
                orders30d     = orders.Count(o => o.CreatedAt >= cutoff),
                activeOrders  = orders.Count(o => o.Status != "Cancelled"),
                totalRevenue, revenue30d,
                totalUsers    = users.Count,
                newUsers30d   = users.Count(u => u.CreatedAt >= cutoff),
                totalLeads    = leads.Count,
                conversionRate = leads.Count > 0
                    ? Math.Round((double)convertedLeads / leads.Count * 100, 1) : 0.0,
                statusBreakdown, topRoutes, leadBreakdown,
            });
        }
    }
}
