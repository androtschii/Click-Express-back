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
    public class ReviewController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ReviewController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll([FromQuery] bool onlyApproved = true)
        {
            var query = _db.Reviews.Include(r => r.User).AsQueryable();
            if (onlyApproved) query = query.Where(r => r.IsApproved);
            var items = query
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new
                {
                    r.Id,
                    r.Rating,
                    r.Text,
                    r.CreatedAt,
                    r.IsApproved,
                    r.ProductId,
                    Username = r.User.Username
                })
                .ToList();
            return Ok(items);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult GetById(int id)
        {
            var review = _db.Reviews.Include(r => r.User).FirstOrDefault(r => r.Id == id);
            if (review == null) return NotFound(new { message = $"Review {id} not found" });
            return Ok(new
            {
                review.Id,
                review.Rating,
                review.Text,
                review.CreatedAt,
                review.IsApproved,
                review.ProductId,
                Username = review.User.Username
            });
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create([FromBody] CreateReviewDto dto)
        {
            if (dto.Rating < 1 || dto.Rating > 5)
                return BadRequest(new { message = "Rating must be between 1 and 5" });
            if (string.IsNullOrWhiteSpace(dto.Text))
                return BadRequest(new { message = "Text is required" });

            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var user = _db.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return Unauthorized();

            var review = new Review
            {
                UserId = user.Id,
                ProductId = dto.ProductId,
                Rating = dto.Rating,
                Text = dto.Text,
                IsApproved = false
            };
            _db.Reviews.Add(review);
            _db.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = review.Id }, new { review.Id });
        }

        [HttpPatch("{id}/approve")]
        [Authorize(Roles = "Admin")]
        public IActionResult Approve(int id)
        {
            var review = _db.Reviews.Find(id);
            if (review == null) return NotFound(new { message = $"Review {id} not found" });
            review.IsApproved = true;
            _db.SaveChanges();
            return Ok(new { review.Id, review.IsApproved });
        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(int id)
        {
            var review = _db.Reviews.Find(id);
            if (review == null) return NotFound(new { message = $"Review {id} not found" });

            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var user = _db.Users.FirstOrDefault(u => u.Username == username);

            if (role != "Admin" && (user == null || review.UserId != user.Id))
                return Forbid();

            _db.Reviews.Remove(review);
            _db.SaveChanges();
            return NoContent();
        }
    }

    public class CreateReviewDto
    {
        public int? ProductId { get; set; }
        public int Rating { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
