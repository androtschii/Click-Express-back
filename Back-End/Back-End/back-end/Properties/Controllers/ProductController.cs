using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using back_end.BLL.DTOs;
using back_end.DAL.Repositories;
using back_end.Domain;
using AutoMapper;

namespace back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _repo;
        private readonly IMapper _mapper;

        public ProductController(IProductRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll([FromQuery] string? search, [FromQuery] string? category)
        {
            var products = _repo.GetAll(search, category);
            return Ok(_mapper.Map<List<ProductDto>>(products));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult GetById(int id)
        {
            var product = _repo.GetById(id);
            if (product == null) return NotFound(new { Message = $"Product {id} not found" });
            return Ok(_mapper.Map<ProductDto>(product));
        }

      
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create([FromBody] CreateProductDto dto)
        {
            if (string.IsNullOrEmpty(dto.Name))
                return BadRequest(new { Message = "Name is required" });
            var product = _mapper.Map<Product>(dto);
            var created = _repo.Create(product);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, _mapper.Map<ProductDto>(created));
        }

      
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id, [FromBody] UpdateProductDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            var updated = _repo.Update(id, product);
            if (updated == null) return NotFound(new { Message = $"Product {id} not found" });
            return Ok(_mapper.Map<ProductDto>(updated));
        }

     
        [HttpPatch("{id}/price")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdatePrice(int id, [FromBody] UpdatePriceDto dto)
        {
            if (dto.Price <= 0)
                return BadRequest(new { Message = "Price must be greater than 0" });
            var updated = _repo.UpdatePrice(id, dto.Price);
            if (updated == null) return NotFound(new { Message = $"Product {id} not found" });
            return Ok(_mapper.Map<ProductDto>(updated));
        }

        
        [HttpPatch("{id}/image")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateImage(int id, [FromBody] UpdateImageDto dto)
        {
            if (string.IsNullOrEmpty(dto.ImageUrl))
                return BadRequest(new { Message = "ImageUrl is required" });
            var updated = _repo.UpdateImage(id, dto.ImageUrl);
            if (updated == null) return NotFound(new { Message = $"Product {id} not found" });
            return Ok(_mapper.Map<ProductDto>(updated));
        }

       
        [HttpPatch("{id}/toggle")]
        [Authorize(Roles = "Admin")]
        public IActionResult ToggleActive(int id)
        {
            var updated = _repo.ToggleActive(id);
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
            var updated = _repo.UpdateStock(id, dto.Quantity);
            if (updated == null) return NotFound(new { Message = $"Product {id} not found" });
            return Ok(_mapper.Map<ProductDto>(updated));
        }

       
        [HttpGet("stats")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetStats()
        {
            return Ok(_repo.GetStats());
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            if (!_repo.Delete(id))
                return NotFound(new { Message = $"Product {id} not found" });
            return NoContent();
        }
    }
}
