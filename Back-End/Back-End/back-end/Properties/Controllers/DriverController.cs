using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using back_end.DAL;
using back_end.Domain;
using back_end.BLL.DTOs;

namespace back_end.Controllers
{
    [Route("api/drivers")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public DriverController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET /api/drivers
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] string? status)
        {
            var query = _context.Drivers
                .Include(d => d.Vehicle)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(d => d.Status == status);

            var drivers = await query.OrderBy(d => d.FullName).ToListAsync();
            return Ok(_mapper.Map<List<DriverDto>>(drivers));
        }

        // GET /api/drivers/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var driver = await _context.Drivers
                .Include(d => d.Vehicle)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (driver == null)
                return NotFound(new { Message = $"Driver {id} not found" });

            return Ok(_mapper.Map<DriverDto>(driver));
        }

        // POST /api/drivers
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateDriverDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.VehicleId.HasValue)
            {
                var vehicle = await _context.Vehicles.FindAsync(dto.VehicleId.Value);
                if (vehicle == null)
                    return NotFound(new { Message = $"Vehicle {dto.VehicleId} not found" });
            }

            var driver = _mapper.Map<Driver>(dto);
            driver.CreatedAt = DateTime.UtcNow;

            _context.Drivers.Add(driver);
            await _context.SaveChangesAsync();

            await _context.Entry(driver).Reference(d => d.Vehicle).LoadAsync();
            return CreatedAtAction(nameof(GetById), new { id = driver.Id }, _mapper.Map<DriverDto>(driver));
        }

        // PUT /api/drivers/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDriverDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var driver = await _context.Drivers.FindAsync(id);
            if (driver == null)
                return NotFound(new { Message = $"Driver {id} not found" });

            if (dto.VehicleId.HasValue)
            {
                var vehicle = await _context.Vehicles.FindAsync(dto.VehicleId.Value);
                if (vehicle == null)
                    return NotFound(new { Message = $"Vehicle {dto.VehicleId} not found" });
            }

            _mapper.Map(dto, driver);
            await _context.SaveChangesAsync();

            await _context.Entry(driver).Reference(d => d.Vehicle).LoadAsync();
            return Ok(_mapper.Map<DriverDto>(driver));
        }

        // PATCH /api/drivers/{id}/status
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PatchStatus(int id, [FromBody] PatchDriverStatusDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var driver = await _context.Drivers.FindAsync(id);
            if (driver == null)
                return NotFound(new { Message = $"Driver {id} not found" });

            driver.Status = dto.Status;
            await _context.SaveChangesAsync();

            return Ok(new { driver.Id, driver.Status });
        }

        // DELETE /api/drivers/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var driver = await _context.Drivers.FindAsync(id);
            if (driver == null)
                return NotFound(new { Message = $"Driver {id} not found" });

            _context.Drivers.Remove(driver);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
