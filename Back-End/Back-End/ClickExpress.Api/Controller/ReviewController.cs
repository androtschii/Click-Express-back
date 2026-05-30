using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using ClickExpress.BusinessLogic.Interfaces;
using ClickExpress.BusinessLogic.Helpers;
using ClickExpress.DataAccess.Context;
using ClickExpress.Domain.Models.Review;

namespace ClickExpress.Api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewActions _reviewActions;
        private readonly ICacheService _cache;
        private readonly ILogger<ReviewController> _logger;

        public ReviewController(IReviewActions reviewActions, ICacheService cache, ILogger<ReviewController> logger)
        {
            _reviewActions = reviewActions;
            _cache = cache;
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
        [AllowAnonymous]
        public IActionResult GetAll([FromQuery] bool onlyApproved = true)
        {
            if (!onlyApproved) return Ok(_reviewActions.GetAllReviewsAction(false));

            var cacheKey = "reviews:approved:all";
            var cached = _cache.Get<object>(cacheKey);
            if (cached != null) return Ok(cached);

            var result = _reviewActions.GetAllReviewsAction(true);
            _cache.Set(cacheKey, (object)result, TimeSpan.FromMinutes(2));
            return Ok(result);
        }

        [HttpGet("paged")]
        [AllowAnonymous]
        public IActionResult GetPaged(
            [FromQuery] bool onlyApproved = true,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? sortBy = null)
            => Ok(_reviewActions.GetReviewsPagedAction(onlyApproved, page, pageSize, sortBy));

        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult GetById(int id)
        {
            var review = _reviewActions.GetReviewByIdAction(id);
            if (review == null) return NotFound(new { message = $"Review {id} not found" });
            return Ok(review);
        }

        [HttpPost]
        [Authorize]
        [EnableRateLimiting("write")]
        public IActionResult Create([FromBody] CreateReviewDTO dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();
            var result = _reviewActions.ResponseCreateReviewAction(userId.Value, dto);
            if (!result.IsSuccess) return BadRequest(new { message = result.Message });
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, new { result.Id });
        }

        [HttpGet("pending/count")]
        [Authorize(Roles = "Admin")]
        public IActionResult PendingCount() => Ok(new { count = _reviewActions.GetPendingCountAction() });

        [HttpPatch("{id}/approve")]
        [Authorize(Roles = "Admin")]
        public IActionResult Approve(int id)
        {
            var result = _reviewActions.ResponseApproveReviewAction(id);
            if (!result.IsSuccess) return NotFound(new { message = result.Message });
            var admin = User.FindFirst(ClaimTypes.Name)?.Value;
            _cache.RemoveByPrefix("reviews:");
            _logger.LogInformation("Admin {Admin} approved review {Id}", admin, id);
            return Ok(new { id, isApproved = true });
        }

        [HttpPatch("{id}/reject")]
        [Authorize(Roles = "Admin")]
        public IActionResult Reject(int id)
        {
            var result = _reviewActions.ResponseRejectReviewAction(id);
            if (!result.IsSuccess) return NotFound(new { message = result.Message });
            var admin = User.FindFirst(ClaimTypes.Name)?.Value;
            _cache.RemoveByPrefix("reviews:");
            _logger.LogInformation("Admin {Admin} rejected review {Id}", admin, id);
            return Ok(new { id, isApproved = false });
        }

        [HttpPut("{id}")]
        [Authorize]
        public IActionResult Update(int id, [FromBody] UpdateReviewDTO dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();
            var result = _reviewActions.ResponseUpdateReviewAction(id, userId.Value, dto);
            if (!result.IsSuccess) return NotFound(new { message = result.Message });
            return Ok(new { id, isApproved = false });
        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(int id)
        {
            var review = _reviewActions.GetReviewByIdAction(id);
            if (review == null) return NotFound(new { message = $"Review {id} not found" });

            var userId = GetUserId();
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (role != "Admin" && (userId == null || review.UserId != userId.Value))
                return Forbid();

            _reviewActions.ResponseDeleteReviewAction(id);
            return NoContent();
        }
    }
}
