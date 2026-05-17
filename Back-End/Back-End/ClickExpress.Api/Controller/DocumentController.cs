using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ClickExpress.DataAccess.Context;
using ClickExpress.Domain.Entities.Document;

namespace ClickExpress.Api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DocumentController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private static readonly HashSet<string> AllowedExtensions =
            new(StringComparer.OrdinalIgnoreCase) { ".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx" };

        public DocumentController(IWebHostEnvironment env)
        {
            _env = env;
        }

        private int? GetUserId()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (username == null) return null;
            using var db = new UserContext();
            return db.Users.FirstOrDefault(u => u.Username == username)?.Id;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] UploadDocumentRequest dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

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

            using var db = new OrderContext();
            var document = new DocumentData
            {
                Type = dto.Type ?? "other",
                Url = $"/uploads/{fileName}",
                OrderId = dto.OrderId,
                UploadedBy = userId.Value,
                UploadedAt = DateTime.UtcNow
            };
            db.Documents.Add(document);
            await db.SaveChangesAsync();

            var saved = await db.Documents.Include(d => d.UploadedByUser).FirstAsync(d => d.Id == document.Id);
            return Ok(new { saved.Id, saved.Type, saved.Url, saved.OrderId, saved.UploadedBy,
                UploadedByUsername = saved.UploadedByUser.Username, saved.UploadedAt });
        }

        [HttpGet("order/{orderId}")]
        public IActionResult GetByOrder(int orderId)
        {
            using var db = new OrderContext();
            var docs = db.Documents.Include(d => d.UploadedByUser)
                .Where(d => d.OrderId == orderId).OrderByDescending(d => d.UploadedAt).ToList();
            return Ok(docs.Select(d => new { d.Id, d.Type, d.Url, d.OrderId, d.UploadedBy,
                UploadedByUsername = d.UploadedByUser.Username, d.UploadedAt }));
        }
    }

    public class UploadDocumentRequest
    {
        public IFormFile? File { get; set; }
        public string? Type { get; set; }
        public int? OrderId { get; set; }
    }
}
