using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using ClickExpress.BusinessLogic.Interfaces;
using ClickExpress.BusinessLogic.Helpers;
using ClickExpress.Domain.Models.Lead;
using ClickExpress.Domain.Models.Base;
using Microsoft.AspNetCore.Http;

namespace ClickExpress.Api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeadController : ControllerBase
    {
        private readonly ILeadActions _leadActions;
        private readonly IEmailService _email;
        private readonly IBackgroundQueue _queue;
        private readonly IAuditLogService _audit;
        private readonly ILogger<LeadController> _logger;

        public LeadController(ILeadActions leadActions, IEmailService email, IBackgroundQueue queue, IAuditLogService audit, ILogger<LeadController> logger)
        {
            _leadActions = leadActions;
            _email = email;
            _queue = queue;
            _audit = audit;
            _logger = logger;
        }

        /// <summary>Submit a quote/contact request. Sends email notifications to admin and customer.</summary>
        [HttpPost]
        [AllowAnonymous]
        [EnableRateLimiting("write")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public IActionResult Submit([FromBody] CreateLeadDTO dto)
        {
            var result = _leadActions.ResponseSubmitLeadAction(dto);

            var fullName = dto.FullName; var email = dto.Email; var phone = dto.Phone;
            var origin = dto.Origin; var destination = dto.Destination;
            var equipment = dto.Equipment; var message = dto.Message;

            _queue.Enqueue(async (sp, ct) =>
            {
                var emailService = sp.GetRequiredService<IEmailService>();
                await emailService.SendLeadAlertAsync(fullName, email, phone, origin, destination, equipment, message);
                if (!string.IsNullOrWhiteSpace(email))
                    await emailService.SendLeadConfirmationAsync(email, fullName, origin, destination, equipment);
            });

            return Ok(new { result.Id, message = "Lead submitted" });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(List<LeadDTO>), StatusCodes.Status200OK)]
        public IActionResult GetAll([FromQuery] string? status) => Ok(_leadActions.GetAllLeadsAction(status));

        [HttpGet("paged")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(PagedResult<LeadDTO>), StatusCodes.Status200OK)]
        public IActionResult GetPaged(
            [FromQuery] string? status,
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 25)
            => Ok(_leadActions.GetLeadsPagedAction(status, search, page, pageSize));

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetById(int id)
        {
            var lead = _leadActions.GetLeadByIdAction(id);
            if (lead == null) return NotFound(new { message = $"Lead {id} not found" });
            return Ok(lead);
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateStatus(int id, [FromBody] UpdateLeadStatusDTO dto)
        {
            var result = _leadActions.ResponseUpdateLeadStatusAction(id, dto.Status);
            if (!result.IsSuccess) return NotFound(new { message = result.Message });
            var admin = User.FindFirst(ClaimTypes.Name)?.Value ?? "unknown";
            _audit.Log("StatusChange", "Lead", id, admin, $"Status → {dto.Status}");
            _logger.LogInformation("Admin {Admin} changed lead {Id} status to {Status}", admin, id, dto.Status);
            return Ok(new { id, status = dto.Status });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var result = _leadActions.ResponseDeleteLeadAction(id);
            if (!result.IsSuccess) return NotFound(new { message = result.Message });
            var admin = User.FindFirst(ClaimTypes.Name)?.Value ?? "unknown";
            _audit.Log("Delete", "Lead", id, admin);
            _logger.LogInformation("Admin {Admin} deleted lead {Id}", admin, id);
            return NoContent();
        }

        [HttpGet("export")]
        [Authorize(Roles = "Admin")]
        public IActionResult Export([FromQuery] string? status)
        {
            var leads = _leadActions.GetAllLeadsAction(status);
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Id,FullName,Email,Phone,Company,Origin,Destination,Equipment,Weight,PickupDate,Status,CreatedAt,Message");
            foreach (var l in leads)
            {
                sb.AppendLine(string.Join(",",
                    l.Id,
                    CsvEscape(l.FullName),
                    CsvEscape(l.Email),
                    CsvEscape(l.Phone),
                    CsvEscape(l.Company ?? ""),
                    CsvEscape(l.Origin),
                    CsvEscape(l.Destination),
                    CsvEscape(l.Equipment),
                    l.Weight?.ToString() ?? "",
                    l.PickupDate?.ToString("yyyy-MM-dd") ?? "",
                    CsvEscape(l.Status),
                    l.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                    CsvEscape(l.Message)));
            }
            var bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", $"leads_{DateTime.UtcNow:yyyyMMdd}.csv");
        }

        private static string CsvEscape(string val)
        {
            if (val.Contains(',') || val.Contains('"') || val.Contains('\n'))
                return "\"" + val.Replace("\"", "\"\"") + "\"";
            return val;
        }
    }
}
