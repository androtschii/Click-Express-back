using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClickExpress.BusinessLogic.Interfaces;
using ClickExpress.Domain.Models.Vehicle;

namespace ClickExpress.Api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleActions _vehicleActions;

        public VehicleController(IVehicleActions vehicleActions)
        {
            _vehicleActions = vehicleActions;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll([FromQuery] string? type, [FromQuery] bool? available)
            => Ok(_vehicleActions.GetAllVehiclesAction(type, available));

        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult GetById(int id)
        {
            var vehicle = _vehicleActions.GetVehicleByIdAction(id);
            if (vehicle == null) return NotFound(new { message = $"Vehicle {id} not found" });
            return Ok(vehicle);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create([FromBody] CreateVehicleDTO dto)
        {
            var result = _vehicleActions.ResponseCreateVehicleAction(dto);
            if (!result.IsSuccess) return BadRequest(new { message = result.Message });
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, _vehicleActions.GetVehicleByIdAction(result.Id));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id, [FromBody] UpdateVehicleDTO dto)
        {
            var result = _vehicleActions.ResponseUpdateVehicleAction(id, dto);
            if (!result.IsSuccess) return NotFound(new { message = result.Message });
            return Ok(_vehicleActions.GetVehicleByIdAction(id));
        }

        [HttpPatch("{id}/availability")]
        [Authorize(Roles = "Admin")]
        public IActionResult ToggleAvailability(int id)
        {
            var result = _vehicleActions.ResponseToggleAvailabilityAction(id);
            if (!result.IsSuccess) return NotFound(new { message = result.Message });
            return Ok(new { message = result.Message });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var result = _vehicleActions.ResponseDeleteVehicleAction(id);
            if (!result.IsSuccess) return NotFound(new { message = result.Message });
            return NoContent();
        }
    }
}
