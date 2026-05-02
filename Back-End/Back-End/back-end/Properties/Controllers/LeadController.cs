using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AutoMapper;
using back_end.BLL.DTOs;
using back_end.DAL;
using back_end.Domain;

namespace back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeadController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly ILogger<LeadController> _logger;

        public LeadController(AppDbContext db, IMapper mapper, ILogger<LeadController> logger)
        {
            _db = db;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Submit([FromBody] CreateLeadDto dto)
        {
            var lead = _mapper.Map<Lead>(dto);
            _db.Leads.Add(lead);
            _db.SaveChanges();
            _logger.LogInformation("Lead {Id} submitted from {Email}: {Origin} -> {Destination}", lead.Id, lead.Email, lead.Origin, lead.Destination);
            return Ok(new { lead.Id, message = "Lead submitted" });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAll([FromQuery] string? status)
        {
            var query = _db.Leads.AsQueryable();
            if (!string.IsNullOrEmpty(status)) query = query.Where(l => l.Status == status);
            var items = query.OrderByDescending(l => l.CreatedAt).ToList();
            return Ok(_mapper.Map<List<LeadDto>>(items));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetById(int id)
        {
            var lead = _db.Leads.Find(id);
            if (lead == null) return NotFound(new { message = $"Lead {id} not found" });
            return Ok(_mapper.Map<LeadDto>(lead));
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateStatus(int id, [FromBody] UpdateLeadStatusDto dto)
        {
            var lead = _db.Leads.Find(id);
            if (lead == null) return NotFound(new { message = $"Lead {id} not found" });
            lead.Status = dto.Status;
            _db.SaveChanges();
            var admin = User.FindFirst(ClaimTypes.Name)?.Value;
            _logger.LogInformation("Admin {Admin} changed lead {LeadId} status to {Status}", admin, id, dto.Status);
            return Ok(new { lead.Id, lead.Status });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var lead = _db.Leads.Find(id);
            if (lead == null) return NotFound(new { message = $"Lead {id} not found" });
            _db.Leads.Remove(lead);
            _db.SaveChanges();
            var admin = User.FindFirst(ClaimTypes.Name)?.Value;
            _logger.LogWarning("Admin {Admin} deleted lead {LeadId}", admin, id);
            return NoContent();
        }
    }
}
