using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using back_end.DAL;
using back_end.Domain;

namespace back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobApplicationController : ControllerBase
    {
        private readonly AppDbContext _db;

        public JobApplicationController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Submit([FromBody] CreateJobApplicationDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FullName) || string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest(new { message = "FullName and Email are required" });
            if (string.IsNullOrWhiteSpace(dto.Position))
                return BadRequest(new { message = "Position is required" });

            var application = new JobApplication
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                Position = dto.Position,
                Message = dto.Message,
                Status = "New"
            };
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
            return Ok(query.OrderByDescending(a => a.CreatedAt).ToList());
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetById(int id)
        {
            var application = _db.JobApplications.Find(id);
            if (application == null) return NotFound(new { message = $"Application {id} not found" });
            return Ok(application);
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateStatus(int id, [FromBody] UpdateStatusDto dto)
        {
            var application = _db.JobApplications.Find(id);
            if (application == null) return NotFound(new { message = $"Application {id} not found" });
            application.Status = dto.Status;
            _db.SaveChanges();
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
            return NoContent();
        }
    }

    public class CreateJobApplicationDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public class UpdateStatusDto
    {
        public string Status { get; set; } = string.Empty;
    }
}
