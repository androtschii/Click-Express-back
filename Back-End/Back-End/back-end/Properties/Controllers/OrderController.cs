using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using back_end.BLL.DTOs;
using back_end.BLL.Services;
using back_end.DAL;
using back_end.Domain;
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
        private readonly ILogger<OrderController> _logger;
        public OrderController(IOrderService orderService, AppDbContext db, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _db = db;
            _logger = logger;
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
        [HttpPost("checkout")]
        public IActionResult Checkout()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var user = _db.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return Unauthorized();

            var cart = _db.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefault(c => c.UserId == user.Id);

            if (cart == null || !cart.Items.Any())
                return BadRequest(new { message = "Cart is empty" });

            var orderIds = new List<int>();
            foreach (var item in cart.Items)
            {
                var order = new Order
                {
                    UserId = user.Id,
                    ProductId = item.ProductId,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow,
                    TotalPrice = item.Product.Price * item.Quantity
                };
                _db.Orders.Add(order);
                _db.SaveChanges();

                _db.OrderItems.Add(new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.Price
                });
                orderIds.Add(order.Id);
            }

            _db.CartItems.RemoveRange(cart.Items);
            _db.SaveChanges();

            var result = orderIds.Select(id => _orderService.GetById(id));
            return Ok(result);
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
        [HttpGet("{id}/tracking")]
        public IActionResult GetTracking(int id)
        {
            var order = _orderService.GetById(id);
            if (order == null) return NotFound(new { Message = $"Order {id} not found" });
            return Ok(_orderService.GetTracking(id));
        }

        [HttpPatch("{id}/cancel")]
        public IActionResult CancelOrder(int id)
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var user = _db.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return Unauthorized();

            var order = _orderService.GetById(id);
            if (order == null) return NotFound(new { Message = $"Order {id} not found" });
            if (order.UserId != user.Id) return Forbid();

            var updated = _orderService.UpdateStatus(id, "Cancelled");
            if (updated == null) return NotFound(new { Message = $"Order {id} not found" });
            return Ok(updated);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id, [FromBody] UpdateOrderDto dto)
        {
            var updated = _orderService.Update(id, dto);
            if (updated == null) return NotFound(new { Message = $"Order {id} not found" });
            return Ok(updated);
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        [AdminActionFilter]
        public IActionResult UpdateStatus(int id, [FromBody] UpdateOrderStatusDto dto)
        {
            var updated = _orderService.UpdateStatus(id, dto.Status);
            if (updated == null) return NotFound(new { Message = $"Order {id} not found" });
            var admin = User.FindFirst(ClaimTypes.Name)?.Value;
            _logger.LogInformation("Admin {Admin} changed order {OrderId} status to {Status}", admin, id, dto.Status);
            return Ok(updated);
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            if (!_orderService.Delete(id))
                return NotFound(new { Message = $"Order {id} not found" });
            var admin = User.FindFirst(ClaimTypes.Name)?.Value;
            _logger.LogWarning("Admin {Admin} deleted order {OrderId}", admin, id);
            return NoContent();
        }
    }
}