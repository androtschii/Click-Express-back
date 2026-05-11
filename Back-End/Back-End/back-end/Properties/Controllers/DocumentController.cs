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
    public class DocumentController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        private static readonly HashSet<string> AllowedExtensions =
            new(StringComparer.OrdinalIgnoreCase) { ".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx" };

        public DocumentController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        private User? GetCurrentUser()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (username == null) return null;
            return _db.Users.FirstOrDefault(u => u.Username == username);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] UploadDocumentDto dto)
        {
            var user = GetCurrentUser();
            if (user == null) return Unauthorized();

            if (dto.File == null || dto.File.Length == 0)
                return BadRequest(new { message = "No file provided" });

            var ext = Path.GetExtension(dto.File.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext))
                return BadRequest(new { message = $"File type '{ext}' not allowed" });

            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadsPath = Path.Combine(webRoot, "uploads");
            Directory.CreateDirectory(uploadsPath);

            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
                await dto.File.CopyToAsync(stream);

            var document = new Document
            {
                Type = dto.Type ?? "other",
                Url = $"/uploads/{fileName}",
                OrderId = dto.OrderId,
                UploadedBy = user.Id,
                UploadedAt = DateTime.UtcNow
            };
            _db.Documents.Add(document);
            await _db.SaveChangesAsync();

            var saved = await _db.Documents
                .Include(d => d.UploadedByUser)
                .FirstAsync(d => d.Id == document.Id);

            return Ok(ToDto(saved));
        }

        [HttpGet("order/{orderId}")]
        public IActionResult GetByOrder(int orderId)
        {
            var docs = _db.Documents
                .Include(d => d.UploadedByUser)
                .Where(d => d.OrderId == orderId)
                .OrderByDescending(d => d.UploadedAt)
                .ToList();

            return Ok(docs.Select(ToDto));
        }

        private static object ToDto(Document d) => new
        {
            d.Id,
            d.Type,
            d.Url,
            d.OrderId,
            d.UploadedBy,
            UploadedByUsername = d.UploadedByUser.Username,
            d.UploadedAt
        };
    }

    public class UploadDocumentDto
    {
        public IFormFile? File { get; set; }
        public string? Type { get; set; }
        public int? OrderId { get; set; }
    }
}
