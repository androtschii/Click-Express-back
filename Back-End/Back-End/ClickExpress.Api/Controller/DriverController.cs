using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ClickExpress.BusinessLogic.Helpers;
using ClickExpress.BusinessLogic.Interfaces;
using ClickExpress.Domain.Models.Driver;

namespace ClickExpress.Api.Controller
{
    [Route("api/drivers")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        private readonly IDriverActions _driverActions;
        private readonly IAuditLogService _audit;

        public DriverController(IDriverActions driverActions, IAuditLogService audit)
        {
            _driverActions = driverActions;
            _audit = audit;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll([FromQuery] string? status) => Ok(_driverActions.GetAllDriversAction(status));

        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult GetById(int id)
        {
            var driver = _driverActions.GetDriverByIdAction(id);
            if (driver == null) return NotFound(new { message = $"Driver {id} not found" });
            return Ok(driver);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create([FromBody] CreateDriverDTO dto)
        {
            var result = _driverActions.ResponseCreateDriverAction(dto);
            if (!result.IsSuccess) return BadRequest(new { message = result.Message });
            var admin = User.FindFirst(ClaimTypes.Name)?.Value ?? "unknown";
            _audit.Log("Create", "Driver", result.Id, admin);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, _driverActions.GetDriverByIdAction(result.Id));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id, [FromBody] UpdateDriverDTO dto)
        {
            var result = _driverActions.ResponseUpdateDriverAction(id, dto);
            if (!result.IsSuccess) return NotFound(new { message = result.Message });
            var admin = User.FindFirst(ClaimTypes.Name)?.Value ?? "unknown";
            _audit.Log("Update", "Driver", id, admin);
            return Ok(_driverActions.GetDriverByIdAction(id));
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public IActionResult PatchStatus(int id, [FromBody] PatchDriverStatusDTO dto)
        {
            var result = _driverActions.ResponsePatchDriverStatusAction(id, dto.Status);
            if (!result.IsSuccess) return NotFound(new { message = result.Message });
            var admin = User.FindFirst(ClaimTypes.Name)?.Value ?? "unknown";
            _audit.Log("StatusChange", "Driver", id, admin, $"Status changed to {dto.Status}");
            return Ok(new { id, status = dto.Status });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var result = _driverActions.ResponseDeleteDriverAction(id);
            if (!result.IsSuccess) return NotFound(new { message = result.Message });
            var admin = User.FindFirst(ClaimTypes.Name)?.Value ?? "unknown";
            _audit.Log("Delete", "Driver", id, admin);
            return NoContent();
        }

        [HttpGet("deleted")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetDeleted() => Ok(_driverActions.GetDeletedDriversAction());

        [HttpPost("{id}/restore")]
        [Authorize(Roles = "Admin")]
        public IActionResult Restore(int id)
        {
            var result = _driverActions.RestoreDriverAction(id);
            if (!result.IsSuccess) return NotFound(new { message = result.Message });
            var admin = User.FindFirst(ClaimTypes.Name)?.Value ?? "unknown";
            _audit.Log("Restore", "Driver", id, admin);
            return Ok(_driverActions.GetDriverByIdAction(id));
        }
    }
}
