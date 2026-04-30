using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using back_end.BLL.DTOs;
using back_end.DAL;
using back_end.Domain;

namespace back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public VehicleController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll([FromQuery] string? type, [FromQuery] bool? available)
        {
            var query = _db.Vehicles.AsQueryable();
            if (!string.IsNullOrEmpty(type)) query = query.Where(v => v.Type == type);
            if (available.HasValue) query = query.Where(v => v.IsAvailable == available.Value);
            var items = query.OrderBy(v => v.Model).ToList();
            return Ok(_mapper.Map<List<VehicleDto>>(items));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult GetById(int id)
        {
            var vehicle = _db.Vehicles.Find(id);
            if (vehicle == null) return NotFound(new { message = $"Vehicle {id} not found" });
            return Ok(_mapper.Map<VehicleDto>(vehicle));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create([FromBody] CreateVehicleDto dto)
        {
            var vehicle = _mapper.Map<Vehicle>(dto);
            _db.Vehicles.Add(vehicle);
            _db.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = vehicle.Id }, _mapper.Map<VehicleDto>(vehicle));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id, [FromBody] UpdateVehicleDto dto)
        {
            var vehicle = _db.Vehicles.Find(id);
            if (vehicle == null) return NotFound(new { message = $"Vehicle {id} not found" });

            _mapper.Map(dto, vehicle);
            _db.SaveChanges();
            return Ok(_mapper.Map<VehicleDto>(vehicle));
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
