using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ClickExpress.BusinessLogic.Interfaces;
using ClickExpress.Domain.Models.Lead;

namespace ClickExpress.Api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeadController : ControllerBase
    {
        private readonly ILeadActions _leadActions;
        private readonly ILogger<LeadController> _logger;

        public LeadController(ILeadActions leadActions, ILogger<LeadController> logger)
        {
            _leadActions = leadActions;
            _logger = logger;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Submit([FromBody] CreateLeadDTO dto)
        {
            var result = _leadActions.ResponseSubmitLeadAction(dto);
            return Ok(new { result.Id, message = "Lead submitted" });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAll([FromQuery] string? status) => Ok(_leadActions.GetAllLeadsAction(status));

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
            var admin = User.FindFirst(ClaimTypes.Name)?.Value;
            _logger.LogInformation("Admin {Admin} changed lead {Id} status to {Status}", admin, id, dto.Status);
            return Ok(new { id, status = dto.Status });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var result = _leadActions.ResponseDeleteLeadAction(id);
            if (!result.IsSuccess) return NotFound(new { message = result.Message });
            return NoContent();
        }
    }
}
