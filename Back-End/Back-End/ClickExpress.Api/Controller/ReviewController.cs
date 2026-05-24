using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ClickExpress.BusinessLogic.Interfaces;
using ClickExpress.DataAccess.Context;
using ClickExpress.Domain.Models.Review;

namespace ClickExpress.Api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewActions _reviewActions;
        private readonly ILogger<ReviewController> _logger;

        public ReviewController(IReviewActions reviewActions, ILogger<ReviewController> logger)
        {
            _reviewActions = reviewActions;
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
        public IActionResult GetAll([FromQuery] bool onlyApproved = true) => Ok(_reviewActions.GetAllReviewsAction(onlyApproved));

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
            _logger.LogInformation("Admin {Admin} rejected review {Id}", admin, id);
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
