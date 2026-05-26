using ClickExpress.DataAccess.Context;
using ClickExpress.Domain.Entities.AuditLog;
using ClickExpress.Domain.Models.AuditLog;

namespace ClickExpress.BusinessLogic.Helpers
{
    public class AuditLogService : IAuditLogService
    {
        public void Log(string action, string entityType, int entityId, string username, string? details = null)
        {
            using var db = new OrderContext();
            db.AuditLogs.Add(new AuditLogData
            {
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                Username = username,
                Details = details,
                Timestamp = DateTime.UtcNow
            });
            db.SaveChanges();
        }

        public (List<AuditLogDTO> Items, int Total) GetLogs(
            string? entityType, string? action, string? username,
            DateTime? from, DateTime? to, int page, int pageSize)
        {
            using var db = new OrderContext();
            var query = db.AuditLogs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(entityType))
                query = query.Where(l => l.EntityType == entityType);
            if (!string.IsNullOrWhiteSpace(action))
                query = query.Where(l => l.Action == action);
            if (!string.IsNullOrWhiteSpace(username))
                query = query.Where(l => l.Username.Contains(username));
            if (from.HasValue)
                query = query.Where(l => l.Timestamp >= from.Value);
            if (to.HasValue)
                query = query.Where(l => l.Timestamp <= to.Value);

            var total = query.Count();
            var items = query
                .OrderByDescending(l => l.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(l => new AuditLogDTO
                {
                    Id = l.Id,
                    Action = l.Action,
                    EntityType = l.EntityType,
                    EntityId = l.EntityId,
                    Username = l.Username,
                    Details = l.Details,
                    Timestamp = l.Timestamp
                })
                .ToList();

            return (items, total);
        }
    }
}
