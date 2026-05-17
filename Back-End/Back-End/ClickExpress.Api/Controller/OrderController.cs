using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ClickExpress.Api.Filters;
using ClickExpress.BusinessLogic.Interfaces;
using ClickExpress.DataAccess.Context;
using ClickExpress.Domain.Entities.Order;
using ClickExpress.Domain.Models.Order;

namespace ClickExpress.Api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderActions _orderActions;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderActions orderActions, ILogger<OrderController> logger)
        {
            _orderActions = orderActions;
            _logger = logger;
        }

        private int? GetUserId()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (username == null) return null;
            using var db = new UserContext();
            return db.Users.FirstOrDefault(u => u.Username == username)?.Id;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAll() => Ok(_orderActions.GetAllOrdersAction());

        [HttpGet("stats")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetStats() => Ok(_orderActions.GetOrderStatsAction());

        [HttpGet("my")]
        public IActionResult GetMy()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();
            return Ok(_orderActions.GetOrdersByUserIdAction(userId.Value));
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var order = _orderActions.GetOrderByIdAction(id);
            if (order == null) return NotFound(new { message = $"Order {id} not found" });
            return Ok(order);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateOrderDTO dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();
            var result = _orderActions.ResponseCreateOrderAction(userId.Value, dto);
            if (!result.IsSuccess) return BadRequest(new { message = result.Message });
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, _orderActions.GetOrderByIdAction(result.Id));
        }

        [HttpPost("checkout")]
        public IActionResult Checkout()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            using var db = new OrderContext();
            var cart = db.Carts.Include(c => c.Items).ThenInclude(i => i.Product)
                .FirstOrDefault(c => c.UserId == userId.Value);

            if (cart == null || !cart.Items.Any())
                return BadRequest(new { message = "Cart is empty" });

            var orderIds = new List<int>();
            foreach (var item in cart.Items)
            {
                var order = new OrderData
                {
                    UserId = userId.Value, ProductId = item.ProductId,
                    Status = "Pending", CreatedAt = DateTime.UtcNow,
                    TotalPrice = item.Product!.Price * item.Quantity
                };
                db.Orders.Add(order);
                db.SaveChanges();

                db.OrderStatusHistories.Add(new OrderStatusHistoryData
                {
                    OrderId = order.Id, Status = "Pending",
                    Timestamp = DateTime.UtcNow, Note = "Order placed via checkout"
                });
                db.OrderItems.Add(new OrderItemData
                {
                    OrderId = order.Id, ProductId = item.ProductId,
                    Quantity = item.Quantity, UnitPrice = item.Product.Price
                });
                orderIds.Add(order.Id);
            }

            db.CartItems.RemoveRange(cart.Items);
            db.SaveChanges();

            return Ok(orderIds.Select(id => _orderActions.GetOrderByIdAction(id)));
        }

        [HttpGet("{id}/tracking")]
        public IActionResult GetTracking(int id)
        {
            var order = _orderActions.GetOrderByIdAction(id);
            if (order == null) return NotFound(new { message = $"Order {id} not found" });
            return Ok(_orderActions.GetOrderTrackingAction(id));
        }

        [HttpPatch("{id}/cancel")]
        public IActionResult CancelOrder(int id)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();
            var order = _orderActions.GetOrderByIdAction(id);
            if (order == null) return NotFound(new { message = $"Order {id} not found" });
            if (order.UserId != userId.Value) return Forbid();
            var result = _orderActions.ResponseUpdateOrderStatusAction(id, "Cancelled");
            if (!result.IsSuccess) return NotFound(new { message = result.Message });
            return Ok(_orderActions.GetOrderByIdAction(id));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id, [FromBody] UpdateOrderDTO dto)
        {
            var result = _orderActions.ResponseUpdateOrderAction(id, dto);
            if (!result.IsSuccess) return NotFound(new { message = result.Message });
            return Ok(_orderActions.GetOrderByIdAction(id));
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        [AdminActionFilter]
        public IActionResult UpdateStatus(int id, [FromBody] UpdateOrderStatusDTO dto)
        {
            var result = _orderActions.ResponseUpdateOrderStatusAction(id, dto.Status);
            if (!result.IsSuccess) return NotFound(new { message = result.Message });
            var admin = User.FindFirst(ClaimTypes.Name)?.Value;
            _logger.LogInformation("Admin {Admin} changed order {Id} status to {Status}", admin, id, dto.Status);
            return Ok(_orderActions.GetOrderByIdAction(id));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var result = _orderActions.ResponseDeleteOrderAction(id);
            if (!result.IsSuccess) return NotFound(new { message = result.Message });
            var admin = User.FindFirst(ClaimTypes.Name)?.Value;
            _logger.LogWarning("Admin {Admin} deleted order {Id}", admin, id);
            return NoContent();
        }
    }
}
