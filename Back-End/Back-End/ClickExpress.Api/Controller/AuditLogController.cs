using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClickExpress.BusinessLogic.Helpers;

namespace ClickExpress.Api.Controller
{
    [Route("api/audit")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AuditLogController : ControllerBase
    {
        private readonly IAuditLogService _audit;

        public AuditLogController(IAuditLogService audit)
        {
            _audit = audit;
        }

        [HttpGet]
        public IActionResult GetLogs(
            [FromQuery] string? entityType,
            [FromQuery] string? action,
            [FromQuery] string? username,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 200) pageSize = 50;

            var validEntityTypes = new[] { "Product", "Driver", "Order" };
            if (!string.IsNullOrWhiteSpace(entityType) && !validEntityTypes.Contains(entityType))
                return BadRequest(new { message = $"Invalid entityType. Allowed: {string.Join(", ", validEntityTypes)}" });

            var validActions = new[] { "Create", "Update", "Delete", "Restore", "StatusChange", "Upload" };
            if (!string.IsNullOrWhiteSpace(action) && !validActions.Contains(action))
                return BadRequest(new { message = $"Invalid action. Allowed: {string.Join(", ", validActions)}" });

            var (items, total) = _audit.GetLogs(entityType, action, username, from, to, page, pageSize);
            return Ok(new
            {
                Total = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)total / pageSize),
                Items = items
            });
        }
    }
}
