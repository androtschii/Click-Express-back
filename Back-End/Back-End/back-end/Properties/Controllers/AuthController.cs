using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using back_end.DAL;

namespace back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _db;

        public AuthController(IConfiguration config, AppDbContext db)
        {
            _config = config;
            _db = db;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = _db.Users.FirstOrDefault(u =>
                u.Username == request.Username && u.IsActive);

            if (user == null)
                return Unauthorized(new { message = "Неверный логин или пароль" });

            bool passwordOk = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!passwordOk)
                return Unauthorized(new { message = "Неверный логин или пароль" });

            var token = GenerateToken(user.Username, user.Role);
            HttpContext.Session.SetString("username", user.Username);
            HttpContext.Session.SetString("role", user.Role);
            return Ok(new { token, username = user.Username, role = user.Role });
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            if (_db.Users.Any(u => u.Username == request.Username))
                return Conflict(new { message = "Пользователь уже существует" });

            if (_db.Users.Any(u => u.Email == request.Email))
                return Conflict(new { message = "Email уже используется" });

            var user = new back_end.Domain.User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = "User",
                IsActive = true
            };

            _db.Users.Add(user);
            _db.SaveChanges();

            var token = GenerateToken(user.Username, user.Role);
            HttpContext.Session.SetString("username", user.Username);
            HttpContext.Session.SetString("role", user.Role);
            return Ok(new { token, username = user.Username, role = user.Role });
        }

        private string GenerateToken(string username, string role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(8),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class RegisterRequest
    {
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
