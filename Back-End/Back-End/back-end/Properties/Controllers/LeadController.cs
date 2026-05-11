using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using back_end.BLL.DTOs;
using back_end.BLL.Services;

namespace back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeadController : ControllerBase
    {
        private readonly ILeadService _leadService;
        private readonly ILogger<LeadController> _logger;

        public LeadController(ILeadService leadService, ILogger<LeadController> logger)
        {
            _leadService = leadService;
            _logger = logger;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Submit([FromBody] CreateLeadDto dto)
        {
            var created = _leadService.Submit(dto);
            return Ok(new { created.Id, message = "Lead submitted" });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAll([FromQuery] string? status)
            => Ok(_leadService.GetAll(status));

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetById(int id)
        {
            var lead = _leadService.GetById(id);
            if (lead == null) return NotFound(new { message = $"Lead {id} not found" });
            return Ok(lead);
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateStatus(int id, [FromBody] UpdateLeadStatusDto dto)
        {
            var updated = _leadService.UpdateStatus(id, dto.Status);
            if (updated == null) return NotFound(new { message = $"Lead {id} not found" });
            var admin = User.FindFirst(ClaimTypes.Name)?.Value;
            _logger.LogInformation("Admin {Admin} changed lead {LeadId} status to {Status}", admin, id, dto.Status);
            return Ok(new { updated.Id, updated.Status });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            if (!_leadService.Delete(id))
                return NotFound(new { message = $"Lead {id} not found" });
            var admin = User.FindFirst(ClaimTypes.Name)?.Value;
            _logger.LogWarning("Admin {Admin} deleted lead {LeadId}", admin, id);
            return NoContent();
        }
    }
}
