using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ClickExpress.BusinessLogic.Interfaces;
using ClickExpress.DataAccess.Context;
using ClickExpress.Domain.Models.Cart;

namespace ClickExpress.Api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartActions _cartActions;

        public CartController(ICartActions cartActions)
        {
            _cartActions = cartActions;
        }

        private int? GetUserId()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (username == null) return null;
            using var db = new UserContext();
            return db.Users.FirstOrDefault(u => u.Username == username)?.Id;
        }

        [HttpGet("my")]
        public IActionResult GetMyCart()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();
            return Ok(_cartActions.GetOrCreateCartAction(userId.Value));
        }

        [HttpPost("items")]
        public IActionResult AddItem([FromBody] AddCartItemDTO dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();
            if (dto.Quantity < 1) return BadRequest(new { message = "Quantity must be at least 1" });
            var result = _cartActions.ResponseAddCartItemAction(userId.Value, dto.ProductId, dto.Quantity);
            if (!result.IsSuccess) return NotFound(new { message = result.Message });
            return Ok(new { message = "Added", cartId = result.Id });
        }

        [HttpDelete("items/{itemId}")]
        public IActionResult RemoveItem(int itemId)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();
            var result = _cartActions.ResponseRemoveCartItemAction(userId.Value, itemId);
            if (!result.IsSuccess) return NotFound(new { message = result.Message });
            return NoContent();
        }

        [HttpDelete("clear")]
        public IActionResult Clear()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();
            _cartActions.ResponseClearCartAction(userId.Value);
            return NoContent();
        }
    }
}
