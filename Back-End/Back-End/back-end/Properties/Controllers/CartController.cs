using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using back_end.BLL.DTOs;
using back_end.BLL.Services;
using back_end.DAL;

namespace back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly AppDbContext _db;

        public CartController(ICartService cartService, AppDbContext db)
        {
            _cartService = cartService;
            _db = db;
        }

        private int? GetUserId()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (username == null) return null;
            return _db.Users.FirstOrDefault(u => u.Username == username)?.Id;
        }

        [HttpGet("my")]
        public IActionResult GetMyCart()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();
            return Ok(_cartService.GetOrCreate(userId.Value));
        }

        [HttpPost("items")]
        public IActionResult AddItem([FromBody] AddCartItemDto dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();
            if (dto.Quantity < 1) return BadRequest(new { message = "Quantity must be >= 1" });

            try
            {
                var cart = _cartService.AddItem(userId.Value, dto.ProductId, dto.Quantity);
                return Ok(new { message = "Added", cartId = cart.Id });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("items/{itemId}")]
        public IActionResult RemoveItem(int itemId)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var cart = _cartService.GetOrCreate(userId.Value);
            if (!cart.Items.Any(i => i.Id == itemId))
                return NotFound(new { message = "Item not found in your cart" });

            _cartService.RemoveItem(itemId);
            return NoContent();
        }

        [HttpDelete("clear")]
        public IActionResult Clear()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();
            _cartService.Clear(userId.Value);
            return NoContent();
        }
    }
}
