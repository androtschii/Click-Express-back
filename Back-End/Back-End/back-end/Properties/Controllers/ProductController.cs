using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using back_end.BLL.DTOs;
using back_end.BLL.Services;

namespace back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll([FromQuery] string? search, [FromQuery] string? category)
        {
            var products = _productService.GetAll(search, category);
            return Ok(products);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult GetById(int id)
        {
            var product = _productService.GetById(id);
            if (product == null) return NotFound(new { Message = $"Product {id} not found" });
            return Ok(product);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create([FromBody] CreateProductDto dto)
        {
            if (string.IsNullOrEmpty(dto.Name))
                return BadRequest(new { Message = "Name is required" });
            var created = _productService.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id, [FromBody] UpdateProductDto dto)
        {
            var updated = _productService.Update(id, dto);
            if (updated == null) return NotFound(new { Message = $"Product {id} not found" });
            return Ok(updated);
        }

        [HttpPatch("{id}/price")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdatePrice(int id, [FromBody] UpdatePriceDto dto)
        {
            if (dto.Price <= 0)
                return BadRequest(new { Message = "Price must be greater than 0" });
            var updated = _productService.UpdatePrice(id, dto.Price);
            if (updated == null) return NotFound(new { Message = $"Product {id} not found" });
            return Ok(updated);
        }

        [HttpPatch("{id}/image")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateImage(int id, [FromBody] UpdateImageDto dto)
        {
            if (string.IsNullOrEmpty(dto.ImageUrl))
                return BadRequest(new { Message = "ImageUrl is required" });
            var updated = _productService.UpdateImage(id, dto.ImageUrl);
            if (updated == null) return NotFound(new { Message = $"Product {id} not found" });
            return Ok(updated);
        }

        [HttpPatch("{id}/toggle")]
        [Authorize(Roles = "Admin")]
        public IActionResult ToggleActive(int id)
        {
            var updated = _productService.ToggleActive(id);
            if (updated == null) return NotFound(new { Message = $"Product {id} not found" });
            return Ok(new
            {
                Id = updated.Id,
                IsActive = updated.IsActive,
                Message = updated.IsActive ? "Товар активирован" : "Товар деактивирован"
            });
        }

        [HttpPatch("{id}/stock")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateStock(int id, [FromBody] UpdateStockDto dto)
        {
            if (dto.Quantity < 0)
                return BadRequest(new { Message = "Quantity cannot be negative" });
            var updated = _productService.UpdateStock(id, dto.Quantity);
            if (updated == null) return NotFound(new { Message = $"Product {id} not found" });
            return Ok(updated);
        }

        [HttpGet("stats")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetStats()
        {
            return Ok(_productService.GetStats());
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            if (!_productService.Delete(id))
                return NotFound(new { Message = $"Product {id} not found" });
            return NoContent();
        }
    }
}
