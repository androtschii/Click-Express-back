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
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;
        private readonly AppDbContext _db;
        private readonly ILogger<NewsController> _logger;

        public NewsController(INewsService newsService, AppDbContext db, ILogger<NewsController> logger)
        {
            _newsService = newsService;
            _db = db;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll([FromQuery] bool onlyPublished = true)
            => Ok(_newsService.GetAll(onlyPublished));

        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult GetById(int id)
        {
            var article = _newsService.GetById(id);
            if (article == null) return NotFound(new { message = $"Article {id} not found" });
            return Ok(article);
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

            var created = _newsService.Create(author.Id, dto);
            _logger.LogInformation("Admin {Admin} published news article {ArticleId} ({Title})", username, created.Id, created.Title);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, new { created.Id });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id, [FromBody] CreateNewsDto dto)
        {
            var updated = _newsService.Update(id, dto);
            if (updated == null) return NotFound(new { message = $"Article {id} not found" });
            var admin = User.FindFirst(ClaimTypes.Name)?.Value;
            _logger.LogInformation("Admin {Admin} updated news article {ArticleId}", admin, id);
            return Ok(new { updated.Id });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            if (!_newsService.Delete(id))
                return NotFound(new { message = $"Article {id} not found" });
            var admin = User.FindFirst(ClaimTypes.Name)?.Value;
            _logger.LogWarning("Admin {Admin} deleted news article {ArticleId}", admin, id);
            return NoContent();
        }
    }
}
