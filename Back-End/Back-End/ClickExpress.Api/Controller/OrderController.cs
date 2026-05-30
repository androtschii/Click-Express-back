using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ClickExpress.Api.Filters;
using ClickExpress.Api.Hubs;
using ClickExpress.BusinessLogic.Helpers;
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
        private readonly IHubContext<OrderHub> _hub;
        private readonly IEmailService _email;
        private readonly IBackgroundQueue _queue;
        private readonly ILogger<OrderController> _logger;
        private readonly IAuditLogService _audit;

        public OrderController(IOrderActions orderActions, IHubContext<OrderHub> hub, IEmailService email, IBackgroundQueue queue, ILogger<OrderController> logger, IAuditLogService audit)
        {
            _orderActions = orderActions;
            _hub = hub;
            _email = email;
            _queue = queue;
            _logger = logger;
            _audit = audit;
        }

        private int? GetUserId()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (username == null) return null;
            using var db = new UserContext();
            return db.Users.FirstOrDefault(u => u.Username == username)?.Id;
        }

        private (string? Email, string Username) GetUserContact(int userId)
        {
            using var db = new UserContext();
            var u = db.Users.FirstOrDefault(u => u.Id == userId);
            return (u?.Email, u?.Username ?? "");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAll() => Ok(_orderActions.GetAllOrdersAction());

        [HttpGet("paged")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetPaged(
            [FromQuery] string? status,
            [FromQuery] int? userId,
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 25)
            => Ok(_orderActions.GetOrdersPagedAction(status, userId, search, page, pageSize));

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
            var order = _orderActions.GetOrderByIdAction(result.Id);
            if (order != null)
            {
                var (email, username) = GetUserContact(userId.Value);
                if (!string.IsNullOrEmpty(email))
                    _ = Task.Run(async () => await _email.SendOrderConfirmationAsync(email, username, order.Id, order.ProductName, order.TotalPrice));
            }
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, order);
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

            var (userEmail, username) = GetUserContact(userId.Value);
            var createdOrders = orderIds.Select(id => _orderActions.GetOrderByIdAction(id)!).ToList();
            if (!string.IsNullOrEmpty(userEmail))
            {
                _ = Task.Run(async () =>
                {
                    foreach (var o in createdOrders)
                        await _email.SendOrderConfirmationAsync(userEmail, username, o.Id, o.ProductName, o.TotalPrice);
                });
            }

            return Ok(createdOrders);
        }

        [HttpGet("{id}/tracking")]
        public IActionResult GetTracking(int id)
        {
            var order = _orderActions.GetOrderByIdAction(id);
            if (order == null) return NotFound(new { message = $"Order {id} not found" });
            return Ok(_orderActions.GetOrderTrackingAction(id));
        }

        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();
            var order = _orderActions.GetOrderByIdAction(id);
            if (order == null) return NotFound(new { message = $"Order {id} not found" });
            if (order.UserId != userId.Value) return Forbid();
            var result = _orderActions.ResponseUpdateOrderStatusAction(id, "Cancelled");
            if (!result.IsSuccess) return NotFound(new { message = result.Message });
            await _hub.Clients.Group($"order-{id}").SendAsync("StatusChanged",
                new { orderId = id, status = "Cancelled", updatedAt = DateTime.UtcNow });
            var (email, username) = GetUserContact(userId.Value);
            if (!string.IsNullOrEmpty(email))
                _ = Task.Run(async () => await _email.SendOrderStatusUpdateAsync(email, username, id, "Cancelled", order.ProductName));
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
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusDTO dto)
        {
            var order = _orderActions.GetOrderByIdAction(id);
            if (order == null) return NotFound(new { message = $"Order {id} not found" });
            var result = _orderActions.ResponseUpdateOrderStatusAction(id, dto.Status);
            if (!result.IsSuccess) return NotFound(new { message = result.Message });
            var admin = User.FindFirst(ClaimTypes.Name)?.Value ?? "unknown";
            _logger.LogInformation("Admin {Admin} changed order {Id} status to {Status}", admin, id, dto.Status);
            _audit.Log("StatusChange", "Order", id, admin, $"Status changed to {dto.Status}");
            await _hub.Clients.Group($"order-{id}").SendAsync("StatusChanged",
                new { orderId = id, status = dto.Status, updatedAt = DateTime.UtcNow });
            var (email, username) = GetUserContact(order.UserId);
            if (!string.IsNullOrEmpty(email))
            {
                var emailVal = email; var usernameVal = username;
                var statusVal = dto.Status; var productName = order.ProductName;
                _queue.Enqueue(async (sp, ct) =>
                {
                    var emailService = sp.GetRequiredService<IEmailService>();
                    await emailService.SendOrderStatusUpdateAsync(emailVal, usernameVal, id, statusVal, productName);
                });
            }
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
