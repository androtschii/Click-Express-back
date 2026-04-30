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
    public class NewsController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ILogger<NewsController> _logger;

        public NewsController(AppDbContext db, ILogger<NewsController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll([FromQuery] bool onlyPublished = true)
        {
            var query = _db.NewsArticles.Include(n => n.Author).AsQueryable();
            if (onlyPublished) query = query.Where(n => n.IsPublished);
            var items = query
                .OrderByDescending(n => n.PublishedAt)
                .Select(n => new
                {
                    n.Id,
                    n.Title,
                    n.Content,
                    n.ImageUrl,
                    n.PublishedAt,
                    n.IsPublished,
                    AuthorName = n.Author.Username
                })
                .ToList();
            return Ok(items);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult GetById(int id)
        {
            var article = _db.NewsArticles.Include(n => n.Author).FirstOrDefault(n => n.Id == id);
            if (article == null) return NotFound(new { message = $"Article {id} not found" });
            return Ok(new
            {
                article.Id,
                article.Title,
                article.Content,
                article.ImageUrl,
                article.PublishedAt,
                article.IsPublished,
                AuthorName = article.Author.Username
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create([FromBody] CreateNewsDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Content))
                return BadRequest(new { message = "Title and Content are required" });

            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var author = _db.Users.FirstOrDefault(u => u.Username == username);
            if (author == null) return Unauthorized();

            var article = new NewsArticle
            {
                Title = dto.Title,
                Content = dto.Content,
                ImageUrl = dto.ImageUrl,
                AuthorId = author.Id,
                IsPublished = dto.IsPublished
            };
            _db.NewsArticles.Add(article);
            _db.SaveChanges();
            _logger.LogInformation("Admin {Admin} published news article {ArticleId} ({Title})", username, article.Id, article.Title);
            return CreatedAtAction(nameof(GetById), new { id = article.Id }, new { article.Id });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id, [FromBody] CreateNewsDto dto)
        {
            var article = _db.NewsArticles.Find(id);
            if (article == null) return NotFound(new { message = $"Article {id} not found" });

            article.Title = dto.Title;
            article.Content = dto.Content;
            article.ImageUrl = dto.ImageUrl;
            article.IsPublished = dto.IsPublished;
            _db.SaveChanges();
            var admin = User.FindFirst(ClaimTypes.Name)?.Value;
            _logger.LogInformation("Admin {Admin} updated news article {ArticleId}", admin, id);
            return Ok(new { article.Id });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var article = _db.NewsArticles.Find(id);
            if (article == null) return NotFound(new { message = $"Article {id} not found" });
            _db.NewsArticles.Remove(article);
            _db.SaveChanges();
            var admin = User.FindFirst(ClaimTypes.Name)?.Value;
            _logger.LogWarning("Admin {Admin} deleted news article {ArticleId}", admin, id);
            return NoContent();
        }
    }

    public class CreateNewsDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public bool IsPublished { get; set; } = true;
    }
}
