using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using back_end.BLL.DTOs;
using back_end.BLL.Services;

namespace back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobApplicationController : ControllerBase
    {
        private readonly IJobApplicationService _jobApplicationService;
        private readonly ILogger<JobApplicationController> _logger;

        public JobApplicationController(IJobApplicationService jobApplicationService, ILogger<JobApplicationController> logger)
        {
            _jobApplicationService = jobApplicationService;
            _logger = logger;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Submit([FromBody] CreateJobApplicationDto dto)
        {
            var created = _jobApplicationService.Submit(dto);
            return Ok(new { created.Id, message = "Application submitted" });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAll([FromQuery] string? status)
            => Ok(_jobApplicationService.GetAll(status));

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetById(int id)
        {
            var application = _jobApplicationService.GetById(id);
            if (application == null) return NotFound(new { message = $"Application {id} not found" });
            return Ok(application);
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateStatus(int id, [FromBody] UpdateJobApplicationStatusDto dto)
        {
            var updated = _jobApplicationService.UpdateStatus(id, dto.Status);
            if (updated == null) return NotFound(new { message = $"Application {id} not found" });
            var admin = User.FindFirst(ClaimTypes.Name)?.Value;
            _logger.LogInformation("Admin {Admin} changed application {AppId} status to {Status}", admin, id, dto.Status);
            return Ok(new { updated.Id, updated.Status });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            if (!_jobApplicationService.Delete(id))
                return NotFound(new { message = $"Application {id} not found" });
            var admin = User.FindFirst(ClaimTypes.Name)?.Value;
            _logger.LogWarning("Admin {Admin} deleted application {AppId}", admin, id);
            return NoContent();
        }
    }
}
