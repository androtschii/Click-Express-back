using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using back_end.DAL;
using back_end.Domain;

namespace back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _db;

        public CartController(AppDbContext db)
        {
            _db = db;
        }

        private User? GetCurrentUser()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (username == null) return null;
            return _db.Users.FirstOrDefault(u => u.Username == username);
        }

        private Cart GetOrCreateCart(int userId)
        {
            var cart = _db.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefault(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _db.Carts.Add(cart);
                _db.SaveChanges();
            }
            return cart;
        }

        [HttpGet("my")]
        public IActionResult GetMyCart()
        {
            var user = GetCurrentUser();
            if (user == null) return Unauthorized();

            var cart = GetOrCreateCart(user.Id);
            return Ok(new
            {
                cart.Id,
                cart.UserId,
                cart.CreatedAt,
                Items = cart.Items.Select(i => new
                {
                    i.Id,
                    i.ProductId,
                    i.Quantity,
                    ProductName = i.Product.Name,
                    Price = i.Product.Price,
                    Total = i.Product.Price * i.Quantity
                })
            });
        }

        [HttpPost("items")]
        public IActionResult AddItem([FromBody] AddCartItemDto dto)
        {
            if (dto.Quantity < 1) return BadRequest(new { message = "Quantity must be >= 1" });

            var user = GetCurrentUser();
            if (user == null) return Unauthorized();

            var product = _db.Products.Find(dto.ProductId);
            if (product == null) return NotFound(new { message = "Product not found" });

            var cart = GetOrCreateCart(user.Id);
            var existing = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);
            if (existing != null)
            {
                existing.Quantity += dto.Quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity
                });
            }
            _db.SaveChanges();
            return Ok(new { message = "Added", cartId = cart.Id });
        }

        [HttpDelete("items/{itemId}")]
        public IActionResult RemoveItem(int itemId)
        {
            var user = GetCurrentUser();
            if (user == null) return Unauthorized();

            var item = _db.CartItems.Include(i => i.Cart).FirstOrDefault(i => i.Id == itemId);
            if (item == null) return NotFound(new { message = "Item not found" });
            if (item.Cart.UserId != user.Id) return Forbid();

            _db.CartItems.Remove(item);
            _db.SaveChanges();
            return NoContent();
        }

        [HttpDelete("clear")]
        public IActionResult Clear()
        {
            var user = GetCurrentUser();
            if (user == null) return Unauthorized();

            var cart = _db.Carts.Include(c => c.Items).FirstOrDefault(c => c.UserId == user.Id);
            if (cart == null) return NoContent();
            _db.CartItems.RemoveRange(cart.Items);
            _db.SaveChanges();
            return NoContent();
        }
    }

    public class AddCartItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
