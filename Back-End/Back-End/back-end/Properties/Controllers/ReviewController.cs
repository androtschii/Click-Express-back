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
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly AppDbContext _db;
        private readonly ILogger<ReviewController> _logger;

        public ReviewController(IReviewService reviewService, AppDbContext db, ILogger<ReviewController> logger)
        {
            _reviewService = reviewService;
            _db = db;
            _logger = logger;
        }

        private int? GetUserId()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (username == null) return null;
            return _db.Users.FirstOrDefault(u => u.Username == username)?.Id;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll([FromQuery] bool onlyApproved = true)
            => Ok(_reviewService.GetAll(onlyApproved));

        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult GetById(int id)
        {
            var review = _reviewService.GetById(id);
            if (review == null) return NotFound(new { message = $"Review {id} not found" });
            return Ok(review);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create([FromBody] CreateReviewDto dto)
        {
            if (dto.Rating < 1 || dto.Rating > 5)
                return BadRequest(new { message = "Rating must be between 1 and 5" });
            if (string.IsNullOrWhiteSpace(dto.Text))
                return BadRequest(new { message = "Text is required" });

            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var created = _reviewService.Create(userId.Value, dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, new { created.Id });
        }

        [HttpPatch("{id}/approve")]
        [Authorize(Roles = "Admin")]
        public IActionResult Approve(int id)
        {
            var approved = _reviewService.Approve(id);
            if (approved == null) return NotFound(new { message = $"Review {id} not found" });
            var admin = User.FindFirst(ClaimTypes.Name)?.Value;
            _logger.LogInformation("Admin {Admin} approved review {ReviewId}", admin, id);
            return Ok(new { approved.Id, approved.IsApproved });
        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(int id)
        {
            var review = _reviewService.GetById(id);
            if (review == null) return NotFound(new { message = $"Review {id} not found" });

            var userId = GetUserId();
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (role != "Admin" && (userId == null || review.UserId != userId.Value))
                return Forbid();

            _reviewService.Delete(id);
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            _logger.LogWarning("{Role} {Username} deleted review {ReviewId}", role, username, id);
            return NoContent();
        }
    }
}
