using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ClickExpress.BusinessLogic.Interfaces;
using ClickExpress.BusinessLogic.Helpers;
using ClickExpress.Domain.Models.JobApplication;

namespace ClickExpress.Api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobApplicationController : ControllerBase
    {
        private readonly IJobApplicationActions _jobApplicationActions;
        private readonly IEmailService _email;
        private readonly ILogger<JobApplicationController> _logger;

        public JobApplicationController(IJobApplicationActions jobApplicationActions, IEmailService email, ILogger<JobApplicationController> logger)
        {
            _jobApplicationActions = jobApplicationActions;
            _email = email;
            _logger = logger;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Submit([FromBody] CreateJobApplicationDTO dto)
        {
            var result = _jobApplicationActions.ResponseSubmitJobApplicationAction(dto);

            _ = Task.Run(async () =>
            {
                await _email.SendJobApplicationAlertAsync(dto.FullName, dto.Email, dto.Phone, dto.Position, dto.Message);
                if (!string.IsNullOrWhiteSpace(dto.Email))
                    await _email.SendJobApplicationConfirmationAsync(dto.Email, dto.FullName, dto.Position);
            });

            return Ok(new { result.Id, message = "Application submitted" });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAll([FromQuery] string? status) => Ok(_jobApplicationActions.GetAllJobApplicationsAction(status));

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetById(int id)
        {
            var app = _jobApplicationActions.GetJobApplicationByIdAction(id);
            if (app == null) return NotFound(new { message = $"Application {id} not found" });
            return Ok(app);
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateStatus(int id, [FromBody] UpdateJobApplicationStatusDTO dto)
        {
            var result = _jobApplicationActions.ResponseUpdateJobApplicationStatusAction(id, dto.Status);
            if (!result.IsSuccess) return NotFound(new { message = result.Message });
            var admin = User.FindFirst(ClaimTypes.Name)?.Value;
            _logger.LogInformation("Admin {Admin} changed application {Id} status to {Status}", admin, id, dto.Status);
            return Ok(new { id, status = dto.Status });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var result = _jobApplicationActions.ResponseDeleteJobApplicationAction(id);
            if (!result.IsSuccess) return NotFound(new { message = result.Message });
            return NoContent();
        }
    }
}
