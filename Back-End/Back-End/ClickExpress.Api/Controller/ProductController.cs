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
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductActions productActions, ILogger<ProductController> logger)
        {
            _productActions = productActions;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll([FromQuery] string? search, [FromQuery] string? category,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;
            var (items, total) = _productActions.GetAllProductsAction(search, category, page, pageSize);
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

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var result = _productActions.ResponseDeleteProductAction(id);
            if (!result.IsSuccess) return NotFound(new { message = result.Message });
            var admin = User.FindFirst(ClaimTypes.Name)?.Value;
            _logger.LogWarning("Admin {Admin} deleted product {Id}", admin, id);
            return NoContent();
        }
    }
}
