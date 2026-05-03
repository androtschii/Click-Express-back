using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using AutoMapper;
using back_end.DAL;
using back_end.Domain;
using back_end.BLL.DTOs;

namespace back_end.Controllers
{
    [Route("api/user/favorites")]
    [ApiController]
    [Authorize]
    public class FavoritesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public FavoritesController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        private async Task<int?> GetCurrentUserIdAsync()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (username == null) return null;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            return user?.Id;
        }

        // GET /api/user/favorites
        [HttpGet]
        public async Task<IActionResult> GetFavorites()
        {
            var userId = await GetCurrentUserIdAsync();
            if (userId == null) return Unauthorized();

            var saved = await _context.SavedLoads
                .Include(sl => sl.Product)
                .Where(sl => sl.UserId == userId)
                .OrderByDescending(sl => sl.CreatedAt)
                .ToListAsync();

            return Ok(_mapper.Map<List<SavedLoadDto>>(saved));
        }

        // POST /api/user/favorites
        [HttpPost]
        public async Task<IActionResult> AddFavorite([FromBody] AddSavedLoadDto dto)
        {
            var userId = await GetCurrentUserIdAsync();
            if (userId == null) return Unauthorized();

            var product = await _context.Products.FindAsync(dto.ProductId);
            if (product == null)
                return NotFound(new { Message = $"Product {dto.ProductId} not found" });

            var alreadyExists = await _context.SavedLoads
                .AnyAsync(sl => sl.UserId == userId && sl.ProductId == dto.ProductId);
            if (alreadyExists)
                return Conflict(new { Message = "Already in favorites" });

            var saved = new SavedLoad
            {
                UserId = userId.Value,
                ProductId = dto.ProductId,
                CreatedAt = DateTime.UtcNow,
            };

            _context.SavedLoads.Add(saved);
            await _context.SaveChangesAsync();

            await _context.Entry(saved).Reference(sl => sl.Product).LoadAsync();
            return CreatedAtAction(nameof(GetFavorites), _mapper.Map<SavedLoadDto>(saved));
        }

        // DELETE /api/user/favorites/{productId}
        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveFavorite(int productId)
        {
            var userId = await GetCurrentUserIdAsync();
            if (userId == null) return Unauthorized();

            var saved = await _context.SavedLoads
                .FirstOrDefaultAsync(sl => sl.UserId == userId && sl.ProductId == productId);
            if (saved == null)
                return NotFound(new { Message = "Favorite not found" });

            _context.SavedLoads.Remove(saved);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
