using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using back_end.BLL.DTOs;
using back_end.BLL.Services;
using back_end.DAL;
using back_end.Filters;
namespace back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly AppDbContext _db;
        public OrderController(IOrderService orderService, AppDbContext db)
        {
            _orderService = orderService;
            _db = db;
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAll() => Ok(_orderService.GetAll());
        [HttpGet("stats")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetStats() => Ok(_orderService.GetStats());
        [HttpGet("my")]
        public IActionResult GetMy()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var user = _db.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return Unauthorized();
            return Ok(_orderService.GetByUserId(user.Id));
        }
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var order = _orderService.GetById(id);
            if (order == null) return NotFound(new { Message = $"Order {id} not found" });
            return Ok(order);
        }
        [HttpPost]
        public IActionResult Create([FromBody] CreateOrderDto dto)
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var user = _db.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return Unauthorized();
            var created = _orderService.Create(user.Id, dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        [AdminActionFilter]
        public IActionResult UpdateStatus(int id, [FromBody] UpdateOrderStatusDto dto)
        {
            var updated = _orderService.UpdateStatus(id, dto.Status);
            if (updated == null) return NotFound(new { Message = $"Order {id} not found" });
            return Ok(updated);
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            if (!_orderService.Delete(id))
                return NotFound(new { Message = $"Order {id} not found" });
            return NoContent();
        }
    }
}