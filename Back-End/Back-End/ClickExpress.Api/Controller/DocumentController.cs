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

        [HttpGet]
        public IActionResult GetMine()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();
            using var db = new OrderContext();
            var docs = db.Documents.Include(d => d.UploadedByUser)
                .Where(d => d.UploadedBy == userId.Value)
                .OrderByDescending(d => d.UploadedAt)
                .ToList();
            return Ok(docs.Select(d => new { d.Id, d.Type, d.Url, d.OrderId, d.UploadedBy,
                UploadedByUsername = d.UploadedByUser.Username, d.UploadedAt,
                FileName = Path.GetFileName(d.Url) }));
        }

        [HttpGet("order/{orderId}")]
        public IActionResult GetByOrder(int orderId)
        {
            using var db = new OrderContext();
            var docs = db.Documents.Include(d => d.UploadedByUser)
                .Where(d => d.OrderId == orderId).OrderByDescending(d => d.UploadedAt).ToList();
            return Ok(docs.Select(d => new { d.Id, d.Type, d.Url, d.OrderId, d.UploadedBy,
                UploadedByUsername = d.UploadedByUser.Username, d.UploadedAt,
                FileName = Path.GetFileName(d.Url) }));
        }

        [HttpGet("{id}/download")]
        public IActionResult Download(int id)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            using var db = new OrderContext();
            var doc = db.Documents.FirstOrDefault(d => d.Id == id && d.UploadedBy == userId.Value);
            if (doc == null) return NotFound(new { message = "Document not found" });

            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var fullPath = Path.Combine(webRoot, doc.Url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (!System.IO.File.Exists(fullPath))
                return NotFound(new { message = "File not found on disk" });

            var ext = Path.GetExtension(fullPath).ToLowerInvariant();
            var contentType = ext switch
            {
                ".pdf"  => "application/pdf",
                ".png"  => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".doc"  => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _       => "application/octet-stream"
            };
            var fileName = Path.GetFileName(fullPath);
            return PhysicalFile(fullPath, contentType, fileName);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            using var db = new OrderContext();
            var doc = db.Documents.FirstOrDefault(d => d.Id == id && d.UploadedBy == userId.Value);
            if (doc == null) return NotFound(new { message = "Document not found" });

            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var fullPath = Path.Combine(webRoot, doc.Url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (System.IO.File.Exists(fullPath))
                System.IO.File.Delete(fullPath);

            db.Documents.Remove(doc);
            db.SaveChanges();
            return NoContent();
        }
    }

    public class UploadDocumentRequest
    {
        public IFormFile? File { get; set; }
        public string? Type { get; set; }
        public int? OrderId { get; set; }
    }
}
