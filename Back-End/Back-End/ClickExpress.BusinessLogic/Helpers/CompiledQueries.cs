using Microsoft.EntityFrameworkCore;
using ClickExpress.DataAccess.Context;
using ClickExpress.Domain.Entities.User;
using ClickExpress.Domain.Entities.Notification;
using ClickExpress.Domain.Entities.Order;

namespace ClickExpress.BusinessLogic.Helpers
{
    /// <summary>
    /// Pre-compiled EF Core queries for hot-path operations.
    /// Compiled queries skip LINQ expression tree compilation on every call,
    /// reducing CPU overhead on high-frequency endpoints.
    /// </summary>
    public static class CompiledQueries
    {
        // Hot path: every authenticated request resolves the current user by username
        public static readonly Func<UserContext, string, UserData?> GetUserByUsername =
            EF.CompileQuery((UserContext db, string username) =>
                db.Users.FirstOrDefault(u => u.Username == username));

        public static readonly Func<UserContext, int, UserData?> GetUserById =
            EF.CompileQuery((UserContext db, int id) =>
                db.Users.FirstOrDefault(u => u.Id == id));

        public static readonly Func<UserContext, string, UserData?> GetActiveUserByEmail =
            EF.CompileQuery((UserContext db, string email) =>
                db.Users.FirstOrDefault(u => u.Email == email && u.IsActive));

        public static readonly Func<UserContext, string, UserData?> GetUserByRefreshToken =
            EF.CompileQuery((UserContext db, string token) =>
                db.Users.FirstOrDefault(u => u.RefreshToken == token));

        // Notification unread count — called on every page load for the bell badge
        public static readonly Func<OrderContext, int, int> GetUnreadNotificationCount =
            EF.CompileQuery((OrderContext db, int userId) =>
                db.Notifications.Count(n => n.UserId == userId && !n.IsRead));

        // Order lookup by tracking code — called from the public tracking endpoint
        public static readonly Func<OrderContext, string, OrderData?> GetOrderByTrackingCode =
            EF.CompileQuery((OrderContext db, string code) =>
                db.Orders.FirstOrDefault(o => o.TrackingCode == code));
    }
}
