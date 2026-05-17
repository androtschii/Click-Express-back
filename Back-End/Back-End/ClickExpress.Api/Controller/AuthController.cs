using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using ClickExpress.BusinessLogic.Helpers;
using ClickExpress.BusinessLogic.Interfaces;
using ClickExpress.DataAccess.Context;
using ClickExpress.Domain.Models.User;

namespace ClickExpress.Api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUserActions _userActions;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IConfiguration config, IUserActions userActions, ILogger<AuthController> logger)
        {
            _config = config;
            _userActions = userActions;
            _logger = logger;
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult Me()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username)) return Unauthorized();
            var user = _userActions.GetUserByUsernameAction(username);
            if (user == null) return Unauthorized();
            return Ok(user);
        }

        [HttpPost("login")]
        [EnableRateLimiting("auth")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var authDto = new UserAuthDTO { Username = request.Username, Password = request.Password };
            var result = _userActions.ResponseValidateLoginAction(authDto);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed login attempt for {Username}", request.Username);
                return Unauthorized(new { message = "Invalid username or password" });
            }

            var user = _userActions.GetUserByIdAction(result.Id)!;
            var token = TokenHelper.GenerateJwtToken(user.Username, user.Role,
                _config["Jwt:Key"]!, _config["Jwt:Issuer"]!, _config["Jwt:Audience"]!);
            var refresh = TokenHelper.GenerateRefreshToken();
            _userActions.ResponseSaveRefreshTokenAction(user.Id, refresh, DateTime.Now.AddDays(7));

            _logger.LogInformation("User {Username} logged in", user.Username);
            return Ok(new { token, refreshToken = refresh, username = user.Username, role = user.Role });
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            var dto = new UserRegDTO
            {
                Username = request.Username,
                Email = request.Email,
                Password = request.Password,
                Role = "User"
            };

            var result = _userActions.ResponseUserCreateAction(dto);
            if (!result.IsSuccess)
                return Conflict(new { message = result.Message });

            var user = _userActions.GetUserByIdAction(result.Id)!;
            var token = TokenHelper.GenerateJwtToken(user.Username, user.Role,
                _config["Jwt:Key"]!, _config["Jwt:Issuer"]!, _config["Jwt:Audience"]!);
            var refresh = TokenHelper.GenerateRefreshToken();
            _userActions.ResponseSaveRefreshTokenAction(user.Id, refresh, DateTime.Now.AddDays(7));

            _logger.LogInformation("New user registered: {Username}", user.Username);
            return Ok(new { token, refreshToken = refresh, username = user.Username, role = user.Role });
        }

        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] RefreshRequest req)
        {
            var user = _userActions.GetUserByRefreshTokenAction(req.RefreshToken);

            using (var db = new UserContext())
            {
                var u = db.Users.FirstOrDefault(u => u.RefreshToken == req.RefreshToken);
                if (u == null || u.RefreshTokenExpiry == null || u.RefreshTokenExpiry < DateTime.Now)
                    return Unauthorized(new { message = "Refresh token is invalid" });

                var newToken = TokenHelper.GenerateJwtToken(u.Username, u.Role,
                    _config["Jwt:Key"]!, _config["Jwt:Issuer"]!, _config["Jwt:Audience"]!);
                var newRefresh = TokenHelper.GenerateRefreshToken();
                u.RefreshToken = newRefresh;
                u.RefreshTokenExpiry = DateTime.Now.AddDays(7);
                db.SaveChanges();

                return Ok(new { token = newToken, refreshToken = newRefresh });
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout([FromBody] RefreshRequest req)
        {
            using (var db = new UserContext())
            {
                var u = db.Users.FirstOrDefault(u => u.RefreshToken == req.RefreshToken);
                if (u != null)
                {
                    u.RefreshToken = null;
                    u.RefreshTokenExpiry = null;
                    db.SaveChanges();
                }
            }
            return Ok(new { message = "Logged out" });
        }

        [HttpPost("forgot-password")]
        [EnableRateLimiting("auth")]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordRequest req)
        {
            using (var db = new UserContext())
            {
                var u = db.Users.FirstOrDefault(u => u.Email == req.Email && u.IsActive);
                if (u != null)
                {
                    var resetToken = TokenHelper.GenerateRefreshToken();
                    u.PasswordResetToken = resetToken;
                    u.PasswordResetTokenExpiry = DateTime.Now.AddHours(1);
                    db.SaveChanges();
                    _logger.LogInformation("Password reset requested for {Email}, token: {Token}", req.Email, resetToken);
                }
            }
            return Ok(new { message = "If the email is registered, a reset link has been sent" });
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Token) || string.IsNullOrWhiteSpace(req.NewPassword))
                return BadRequest(new { message = "Token and password are required" });

            if (req.NewPassword.Length < 6)
                return BadRequest(new { message = "Password must be at least 6 characters" });

            using (var db = new UserContext())
            {
                var u = db.Users.FirstOrDefault(u => u.PasswordResetToken == req.Token);
                if (u == null || u.PasswordResetTokenExpiry == null || u.PasswordResetTokenExpiry < DateTime.Now)
                    return BadRequest(new { message = "Reset link is invalid or expired" });

                u.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.NewPassword);
                u.PasswordResetToken = null;
                u.PasswordResetTokenExpiry = null;
                u.RefreshToken = null;
                u.RefreshTokenExpiry = null;
                db.SaveChanges();
            }
            return Ok(new { message = "Password updated" });
        }
    }

    public class LoginRequest { public string Username { get; set; } = ""; public string Password { get; set; } = ""; }
    public class RegisterRequest { public string Username { get; set; } = ""; public string Email { get; set; } = ""; public string Password { get; set; } = ""; }
    public class RefreshRequest { public string RefreshToken { get; set; } = ""; }
    public class ForgotPasswordRequest { public string Email { get; set; } = ""; }
    public class ResetPasswordRequest { public string Token { get; set; } = ""; public string NewPassword { get; set; } = ""; }
}
