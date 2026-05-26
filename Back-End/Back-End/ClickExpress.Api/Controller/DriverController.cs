using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClickExpress.BusinessLogic.Interfaces;
using ClickExpress.Domain.Models.Driver;

namespace ClickExpress.Api.Controller
{
    [Route("api/drivers")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        private readonly IDriverActions _driverActions;

        public DriverController(IDriverActions driverActions)
        {
            _driverActions = driverActions;
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
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, _driverActions.GetDriverByIdAction(result.Id));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id, [FromBody] UpdateDriverDTO dto)
        {
            var result = _driverActions.ResponseUpdateDriverAction(id, dto);
            if (!result.IsSuccess) return NotFound(new { message = result.Message });
            return Ok(_driverActions.GetDriverByIdAction(id));
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public IActionResult PatchStatus(int id, [FromBody] PatchDriverStatusDTO dto)
        {
            var result = _driverActions.ResponsePatchDriverStatusAction(id, dto.Status);
            if (!result.IsSuccess) return NotFound(new { message = result.Message });
            return Ok(new { id, status = dto.Status });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var result = _driverActions.ResponseDeleteDriverAction(id);
            if (!result.IsSuccess) return NotFound(new { message = result.Message });
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
            return Ok(_driverActions.GetDriverByIdAction(id));
        }
    }
}
