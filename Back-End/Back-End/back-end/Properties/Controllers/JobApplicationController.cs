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
    public class JobApplicationController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly ILogger<JobApplicationController> _logger;

        public JobApplicationController(AppDbContext db, IMapper mapper, ILogger<JobApplicationController> logger)
        {
            _db = db;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Submit([FromBody] CreateJobApplicationDto dto)
        {
            var application = _mapper.Map<JobApplication>(dto);
            _db.JobApplications.Add(application);
            _db.SaveChanges();
            return Ok(new { application.Id, message = "Application submitted" });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAll([FromQuery] string? status)
        {
            var query = _db.JobApplications.AsQueryable();
            if (!string.IsNullOrEmpty(status)) query = query.Where(a => a.Status == status);
            var items = query.OrderByDescending(a => a.CreatedAt).ToList();
            return Ok(_mapper.Map<List<JobApplicationDto>>(items));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetById(int id)
        {
            var application = _db.JobApplications.Find(id);
            if (application == null) return NotFound(new { message = $"Application {id} not found" });
            return Ok(_mapper.Map<JobApplicationDto>(application));
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateStatus(int id, [FromBody] UpdateJobApplicationStatusDto dto)
        {
            var application = _db.JobApplications.Find(id);
            if (application == null) return NotFound(new { message = $"Application {id} not found" });
            application.Status = dto.Status;
            _db.SaveChanges();
            var admin = User.FindFirst(ClaimTypes.Name)?.Value;
            _logger.LogInformation("Admin {Admin} changed application {AppId} status to {Status}", admin, id, dto.Status);
            return Ok(new { application.Id, application.Status });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var application = _db.JobApplications.Find(id);
            if (application == null) return NotFound(new { message = $"Application {id} not found" });
            _db.JobApplications.Remove(application);
            _db.SaveChanges();
            var admin = User.FindFirst(ClaimTypes.Name)?.Value;
            _logger.LogWarning("Admin {Admin} deleted application {AppId}", admin, id);
            return NoContent();
        }
    }
}
