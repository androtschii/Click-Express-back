using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ClickExpress.BusinessLogic.Interfaces;
using ClickExpress.BusinessLogic.Helpers;
using ClickExpress.DataAccess.Context;
using ClickExpress.Domain.Models.News;

namespace ClickExpress.Api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly INewsActions _newsActions;
        private readonly ICacheService _cache;
        private readonly ILogger<NewsController> _logger;

        public NewsController(INewsActions newsActions, ICacheService cache, ILogger<NewsController> logger)
        {
            _newsActions = newsActions;
            _cache = cache;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll([FromQuery] bool onlyPublished = true)
        {
            if (!onlyPublished) return Ok(_newsActions.GetAllNewsAction(false));

            const string cacheKey = "news:published:all";
            var cached = _cache.Get<object>(cacheKey);
            if (cached != null) return Ok(cached);

            var result = _newsActions.GetAllNewsAction(true);
            _cache.Set(cacheKey, (object)result, TimeSpan.FromMinutes(5));
            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult GetById(int id)
        {
            var cacheKey = $"news:article:{id}";
            var cached = _cache.Get<object>(cacheKey);
            if (cached != null) return Ok(cached);

            var article = _newsActions.GetNewsByIdAction(id);
            if (article == null) return NotFound(new { message = $"Article {id} not found" });

            _cache.Set(cacheKey, (object)article, TimeSpan.FromMinutes(5));
            return Ok(article);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create([FromBody] CreateNewsDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Content))
                return BadRequest(new { message = "Title and Content are required" });

            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            int authorId;
            using (var db = new UserContext())
            {
                var author = db.Users.FirstOrDefault(u => u.Username == username);
                if (author == null) return Unauthorized();
                authorId = author.Id;
            }

            var result = _newsActions.ResponseCreateNewsAction(authorId, dto);
            _cache.RemoveByPrefix("news:");
            _logger.LogInformation("Admin {Admin} created news article {Id}", username, result.Id);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, new { result.Id });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id, [FromBody] CreateNewsDTO dto)
        {
            var result = _newsActions.ResponseUpdateNewsAction(id, dto);
            if (!result.IsSuccess) return NotFound(new { message = result.Message });

            var admin = User.FindFirst(ClaimTypes.Name)?.Value;
            _cache.RemoveByPrefix("news:");
            _logger.LogInformation("Admin {Admin} updated news article {Id}", admin, id);
            return Ok(new { id });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var result = _newsActions.ResponseDeleteNewsAction(id);
            if (!result.IsSuccess) return NotFound(new { message = result.Message });

            _cache.RemoveByPrefix("news:");
            return NoContent();
        }
    }
}
