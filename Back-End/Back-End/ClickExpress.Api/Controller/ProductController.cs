using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ClickExpress.BusinessLogic.Interfaces;
using ClickExpress.Domain.Models.Product;

namespace ClickExpress.Api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductActions _productActions;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ProductController> _logger;

        private static readonly HashSet<string> AllowedImageExtensions =
            new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private const long MaxImageSize = 5 * 1024 * 1024; // 5 MB

        public ProductController(IProductActions productActions, IWebHostEnvironment env, ILogger<ProductController> logger)
        {
            _productActions = productActions;
            _env = env;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll(
            [FromQuery] string? search,
            [FromQuery] string? category,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] string? sortBy,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var validSorts = new[] { "newest", "oldest", "price_asc", "price_desc", "popular" };
            if (!string.IsNullOrWhiteSpace(sortBy) && !validSorts.Contains(sortBy))
                return BadRequest(new { message = $"Invalid sortBy. Allowed: {string.Join(", ", validSorts)}" });

            var (items, total) = _productActions.GetAllProductsAction(search, category, minPrice, maxPrice, sortBy, page, pageSize);
            return Ok(new
            {
                Total = total, Page = page, PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)total / pageSize),
                Items = items
            });
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult GetById(int id)
        {
            var product = _productActions.GetProductByIdAction(id);
            if (product == null) return NotFound(new { message = $"Product {id} not found" });
            return Ok(product);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create([FromBody] CreateProductDTO dto)
        {
            var result = _productActions.ResponseCreateProductAction(dto);
            if (!result.IsSuccess) return BadRequest(new { message = result.Message });
            var admin = User.FindFirst(ClaimTypes.Name)?.Value;
            _logger.LogInformation("Admin {Admin} created product {Id}", admin, result.Id);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, _productActions.GetProductByIdAction(result.Id));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id, [FromBody] UpdateProductDTO dto)
        {
            var result = _productActions.ResponseUpdateProductAction(id, dto);
            if (!result.IsSuccess) return NotFound(new { message = result.Message });
            return Ok(_productActions.GetProductByIdAction(id));
        }

        [HttpPatch("{id}/price")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdatePrice(int id, [FromBody] UpdatePriceDTO dto)
        {
            var result = _productActions.ResponseUpdatePriceAction(id, dto.Price);
            if (!result.IsSuccess) return NotFound(new { message = result.Message });
            return Ok(_productActions.GetProductByIdAction(id));
        }

        [HttpPatch("{id}/image")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateImage(int id, [FromBody] UpdateImageDTO dto)
        {
            var result = _productActions.ResponseUpdateImageAction(id, dto.ImageUrl);
            if (!result.IsSuccess) return NotFound(new { message = result.Message });
            return Ok(_productActions.GetProductByIdAction(id));
        }

        [HttpPatch("{id}/toggle")]
        [Authorize(Roles = "Admin")]
        public IActionResult ToggleActive(int id)
        {
            var result = _productActions.ResponseToggleActiveAction(id);
            if (!result.IsSuccess) return NotFound(new { message = result.Message });
            return Ok(new { message = result.Message });
        }

        [HttpPatch("{id}/stock")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateStock(int id, [FromBody] UpdateStockDTO dto)
        {
            var result = _productActions.ResponseUpdateStockAction(id, dto.Quantity);
            if (!result.IsSuccess) return NotFound(new { message = result.Message });
            return Ok(_productActions.GetProductByIdAction(id));
        }

        [HttpGet("stats")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetStats() => Ok(_productActions.GetProductStatsAction());

        [HttpPost("{id}/view")]
        [AllowAnonymous]
        public IActionResult IncrementView(int id)
        {
            _productActions.ResponseIncrementViewAction(id);
            return NoContent();
        }

        [HttpPatch("{id}/upload-image")]
        [Authorize(Roles = "Admin")]
        [RequestSizeLimit(6 * 1024 * 1024)]
        public async Task<IActionResult> UploadImage(int id, [FromForm] IFormFile file)
        {
            var product = _productActions.GetProductByIdAction(id);
            if (product == null) return NotFound(new { message = $"Product {id} not found" });

            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file provided" });

            if (file.Length > MaxImageSize)
                return BadRequest(new { message = "File exceeds 5 MB limit" });

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedImageExtensions.Contains(ext))
                return BadRequest(new { message = $"File type '{ext}' not allowed. Use jpg, jpeg, png, gif or webp." });

            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var folder  = Path.Combine(webRoot, "uploads", "products");
            Directory.CreateDirectory(folder);

            // Delete previous uploaded file to avoid orphaned files
            if (!string.IsNullOrEmpty(product.ImageUrl) && product.ImageUrl.Contains("/uploads/products/"))
            {
                var oldRelative = product.ImageUrl
                    .Replace($"{Request.Scheme}://{Request.Host}", "")
                    .TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
                var oldPath = Path.Combine(webRoot, oldRelative);
                if (System.IO.File.Exists(oldPath))
                    System.IO.File.Delete(oldPath);
            }

            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(stream);

            var imageUrl = $"{Request.Scheme}://{Request.Host}/uploads/products/{fileName}";
            _productActions.ResponseUpdateImageAction(id, imageUrl);

            var admin = User.FindFirst(ClaimTypes.Name)?.Value;
            _logger.LogInformation("Admin {Admin} uploaded image for product {Id}: {Url}", admin, id, imageUrl);

            return Ok(_productActions.GetProductByIdAction(id));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var result = _productActions.ResponseDeleteProductAction(id);
            if (!result.IsSuccess) return NotFound(new { message = result.Message });
            var admin = User.FindFirst(ClaimTypes.Name)?.Value;
            _logger.LogWarning("Admin {Admin} soft-deleted product {Id}", admin, id);
            return NoContent();
        }

        [HttpGet("deleted")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetDeleted() => Ok(_productActions.GetDeletedProductsAction());

        [HttpPost("{id}/restore")]
        [Authorize(Roles = "Admin")]
        public IActionResult Restore(int id)
        {
            var result = _productActions.RestoreProductAction(id);
            if (!result.IsSuccess) return NotFound(new { message = result.Message });
            var admin = User.FindFirst(ClaimTypes.Name)?.Value;
            _logger.LogInformation("Admin {Admin} restored product {Id}", admin, id);
            return Ok(_productActions.GetProductByIdAction(id));
        }
    }
}
