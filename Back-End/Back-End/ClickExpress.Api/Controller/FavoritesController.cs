using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ClickExpress.BusinessLogic.Interfaces;
using ClickExpress.DataAccess.Context;
using ClickExpress.Domain.Models.SavedLoad;

namespace ClickExpress.Api.Controller
{
    [Route("api/user/favorites")]
    [ApiController]
    [Authorize]
    public class FavoritesController : ControllerBase
    {
        private readonly ISavedLoadActions _savedLoadActions;

        public FavoritesController(ISavedLoadActions savedLoadActions)
        {
            _savedLoadActions = savedLoadActions;
        }

        private int? GetUserId()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (username == null) return null;
            using var db = new UserContext();
            return db.Users.FirstOrDefault(u => u.Username == username)?.Id;
        }

        [HttpGet]
        public IActionResult GetFavorites()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();
            return Ok(_savedLoadActions.GetSavedLoadsAction(userId.Value));
        }

        [HttpPost]
        public IActionResult AddFavorite([FromBody] AddSavedLoadDTO dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();
            var result = _savedLoadActions.ResponseAddSavedLoadAction(userId.Value, dto.ProductId);
            if (!result.IsSuccess) return Conflict(new { message = result.Message });
            return CreatedAtAction(nameof(GetFavorites), _savedLoadActions.GetSavedLoadsAction(userId.Value).FirstOrDefault(s => s.Id == result.Id));
        }

        [HttpDelete("{productId}")]
        public IActionResult RemoveFavorite(int productId)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();
            var result = _savedLoadActions.ResponseRemoveSavedLoadAction(userId.Value, productId);
            if (!result.IsSuccess) return NotFound(new { message = result.Message });
            return NoContent();
        }
    }
}
