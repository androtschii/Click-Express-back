using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ClickExpress.BusinessLogic.Interfaces;
using ClickExpress.BusinessLogic.Helpers;
using ClickExpress.Domain.Models.User;

namespace ClickExpress.Api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserActions _userActions;
        private readonly IAuditLogService _audit;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserActions userActions, IAuditLogService audit, ILogger<UserController> logger)
        {
            _userActions = userActions;
            _audit = audit;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAll() => Ok(_userActions.GetAllUsersAction());

        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (username == null) return Unauthorized();
            var user = _userActions.GetUserByUsernameAction(username);
            if (user == null) return NotFound(new { message = "User not found" });
            return Ok(user);
        }

        [HttpPut("profile")]
        public IActionResult UpdateProfile([FromBody] UpdateProfileDTO dto)
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (username == null) return Unauthorized();
            var result = _userActions.ResponseUserUpdateProfileAction(username, dto);
            if (!result.IsSuccess) return NotFound(new { message = result.Message });
            _logger.LogInformation("User {Username} updated profile", username);
            return Ok(_userActions.GetUserByUsernameAction(username));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetById(int id)
        {
            var user = _userActions.GetUserByIdAction(id);
            if (user == null) return NotFound(new { message = $"User {id} not found" });
            return Ok(user);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create([FromBody] UserRegDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username))
                return BadRequest(new { message = "Username is required" });

            var result = _userActions.ResponseUserCreateAction(dto);
            if (!result.IsSuccess) return BadRequest(new { message = result.Message });

            var admin = User.FindFirst(ClaimTypes.Name)?.Value ?? "unknown";
            _audit.Log("Create", "User", result.Id, admin, $"Created user '{dto.Username}'");
            _logger.LogInformation("Admin {Admin} created user {Username}", admin, dto.Username);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, _userActions.GetUserByIdAction(result.Id));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id, [FromBody] UpdateUserDTO dto)
        {
            var result = _userActions.ResponseUserUpdateAction(id, dto);
            if (!result.IsSuccess) return NotFound(new { message = result.Message });

            var admin = User.FindFirst(ClaimTypes.Name)?.Value ?? "unknown";
            _audit.Log("Update", "User", id, admin);
            return Ok(_userActions.GetUserByIdAction(id));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var result = _userActions.ResponseUserDeleteAction(id);
            if (!result.IsSuccess) return NotFound(new { message = result.Message });

            var admin = User.FindFirst(ClaimTypes.Name)?.Value ?? "unknown";
            _audit.Log("Delete", "User", id, admin);
            _logger.LogWarning("Admin {Admin} deleted user {Id}", admin, id);
            return NoContent();
        }
    }
}
