using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using back_end.DAL;
using back_end.Domain;

namespace back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly AppDbContext _db;

        public VehicleController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll([FromQuery] string? type, [FromQuery] bool? available)
        {
            var query = _db.Vehicles.AsQueryable();
            if (!string.IsNullOrEmpty(type)) query = query.Where(v => v.Type == type);
            if (available.HasValue) query = query.Where(v => v.IsAvailable == available.Value);
            return Ok(query.OrderBy(v => v.Model).ToList());
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult GetById(int id)
        {
            var vehicle = _db.Vehicles.Find(id);
            if (vehicle == null) return NotFound(new { message = $"Vehicle {id} not found" });
            return Ok(vehicle);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create([FromBody] Vehicle vehicle)
        {
            if (string.IsNullOrWhiteSpace(vehicle.Model) || string.IsNullOrWhiteSpace(vehicle.PlateNumber))
                return BadRequest(new { message = "Model and PlateNumber are required" });
            _db.Vehicles.Add(vehicle);
            _db.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = vehicle.Id }, vehicle);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id, [FromBody] Vehicle update)
        {
            var vehicle = _db.Vehicles.Find(id);
            if (vehicle == null) return NotFound(new { message = $"Vehicle {id} not found" });

            vehicle.Model = update.Model;
            vehicle.PlateNumber = update.PlateNumber;
            vehicle.Type = update.Type;
            vehicle.Capacity = update.Capacity;
            vehicle.Year = update.Year;
            vehicle.IsAvailable = update.IsAvailable;
            vehicle.ImageUrl = update.ImageUrl;

            _db.SaveChanges();
            return Ok(vehicle);
        }

        [HttpPatch("{id}/availability")]
        [Authorize(Roles = "Admin")]
        public IActionResult ToggleAvailability(int id)
        {
            var vehicle = _db.Vehicles.Find(id);
            if (vehicle == null) return NotFound(new { message = $"Vehicle {id} not found" });
            vehicle.IsAvailable = !vehicle.IsAvailable;
            _db.SaveChanges();
            return Ok(new { vehicle.Id, vehicle.IsAvailable });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var vehicle = _db.Vehicles.Find(id);
            if (vehicle == null) return NotFound(new { message = $"Vehicle {id} not found" });
            _db.Vehicles.Remove(vehicle);
            _db.SaveChanges();
            return NoContent();
        }
    }
}
