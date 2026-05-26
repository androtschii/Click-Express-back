using ClickExpress.Domain.Models.AuditLog;

namespace ClickExpress.BusinessLogic.Helpers
{
    public interface IAuditLogService
    {
        void Log(string action, string entityType, int entityId, string username, string? details = null);
        (List<AuditLogDTO> Items, int Total) GetLogs(string? entityType, string? action, string? username, DateTime? from, DateTime? to, int page, int pageSize);
    }
}
