using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using back_end.BLL.DTOs;
using back_end.BLL.Services;

namespace back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // User и Admin — просто [Authorize]
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_userService.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = _userService.GetById(id);
            if (user == null) return NotFound(new { Message = $"User {id} not found" });
            return Ok(user);
        }

        // Только Admin
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create([FromBody] CreateUserDto dto)
        {
            if (string.IsNullOrEmpty(dto.Username))
                return BadRequest(new { Message = "Username is required" });
            var created = _userService.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id, [FromBody] UpdateUserDto dto)
        {
            var updated = _userService.Update(id, dto);
            if (updated == null) return NotFound(new { Message = $"User {id} not found" });
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            if (!_userService.Delete(id))
                return NotFound(new { Message = $"User {id} not found" });
            return NoContent();
        }
    }
}
