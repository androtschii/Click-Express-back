using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using back_end.BLL.DTOs;
using back_end.BLL.Services;

namespace back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

        public VehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll([FromQuery] string? type, [FromQuery] bool? available)
            => Ok(_vehicleService.GetAll(type, available));

        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult GetById(int id)
        {
            var vehicle = _vehicleService.GetById(id);
            if (vehicle == null) return NotFound(new { message = $"Vehicle {id} not found" });
            return Ok(vehicle);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create([FromBody] CreateVehicleDto dto)
        {
            var created = _vehicleService.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id, [FromBody] UpdateVehicleDto dto)
        {
            var updated = _vehicleService.Update(id, dto);
            if (updated == null) return NotFound(new { message = $"Vehicle {id} not found" });
            return Ok(updated);
        }

        [HttpPatch("{id}/availability")]
        [Authorize(Roles = "Admin")]
        public IActionResult ToggleAvailability(int id)
        {
            var updated = _vehicleService.ToggleAvailability(id);
            if (updated == null) return NotFound(new { message = $"Vehicle {id} not found" });
            return Ok(new { updated.Id, updated.IsAvailable });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            if (!_vehicleService.Delete(id))
                return NotFound(new { message = $"Vehicle {id} not found" });
            return NoContent();
        }
    }
}
