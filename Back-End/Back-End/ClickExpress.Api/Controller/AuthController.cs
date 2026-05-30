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
        private readonly IEmailService _email;
        private readonly IBackgroundQueue _queue;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IConfiguration config, IUserActions userActions, IEmailService email, IBackgroundQueue queue, ILogger<AuthController> logger)
        {
            _config = config;
            _userActions = userActions;
            _email = email;
            _queue = queue;
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
            _userActions.ResponseSaveRefreshTokenAction(user.Id, refresh, DateTime.UtcNow.AddDays(7));

            _logger.LogInformation("User {Username} logged in", user.Username);
            return Ok(new { token, refreshToken = refresh, username = user.Username, role = user.Role });
        }

        [HttpPost("register")]
        [EnableRateLimiting("register")]
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
            _userActions.ResponseSaveRefreshTokenAction(user.Id, refresh, DateTime.UtcNow.AddDays(7));

            _logger.LogInformation("New user registered: {Username}", user.Username);
            return Ok(new { token, refreshToken = refresh, username = user.Username, role = user.Role });
        }

        [HttpPost("refresh")]
        [EnableRateLimiting("refresh")]
        public IActionResult Refresh([FromBody] RefreshRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.RefreshToken))
                return BadRequest(new { message = "Refresh token is required" });

            using var db = new UserContext();
            var u = db.Users.FirstOrDefault(u => u.RefreshToken == req.RefreshToken);
            if (u == null || u.RefreshTokenExpiry == null || u.RefreshTokenExpiry < DateTime.UtcNow)
                return Unauthorized(new { message = "Refresh token is invalid or expired" });

            var newToken = TokenHelper.GenerateJwtToken(u.Username, u.Role,
                _config["Jwt:Key"]!, _config["Jwt:Issuer"]!, _config["Jwt:Audience"]!);
            var newRefresh = TokenHelper.GenerateRefreshToken();
            _userActions.ResponseSaveRefreshTokenAction(u.Id, newRefresh, DateTime.UtcNow.AddDays(7));

            _logger.LogInformation("Token refreshed for {Username}", u.Username);
            return Ok(new { token = newToken, refreshToken = newRefresh, username = u.Username, role = u.Role });
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
                    u.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);
                    db.SaveChanges();

                    var appUrl = _config["AppUrl"] ?? "http://localhost:5174";
                    var resetLink = $"{appUrl}/reset-password?token={resetToken}";
                    var emailTo = u.Email; var link = resetLink;
                    _queue.Enqueue(async (sp, ct) =>
                    {
                        var emailService = sp.GetRequiredService<IEmailService>();
                        await emailService.SendPasswordResetAsync(emailTo, link);
                    });
                    _logger.LogInformation("Password reset requested for {Email}", req.Email);
                }
            }
            return Ok(new { message = "If the email is registered, a reset link has been sent" });
        }

        [HttpPost("change-password")]
        [Authorize]
        public IActionResult ChangePassword([FromBody] ChangePasswordRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.CurrentPassword) || string.IsNullOrWhiteSpace(req.NewPassword))
                return BadRequest(new { message = "Both passwords are required" });

            if (req.NewPassword.Length < 8)
                return BadRequest(new { message = "New password must be at least 8 characters" });

            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (username == null) return Unauthorized();

            using (var db = new UserContext())
            {
                var u = db.Users.FirstOrDefault(u => u.Username == username && u.IsActive);
                if (u == null) return Unauthorized();

                if (!BCrypt.Net.BCrypt.Verify(req.CurrentPassword, u.PasswordHash))
                    return BadRequest(new { message = "Current password is incorrect" });

                u.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.NewPassword);
                u.RefreshToken = null;
                u.RefreshTokenExpiry = null;
                db.SaveChanges();

                _logger.LogInformation("User {Username} changed password", username);
            }
            return Ok(new { message = "Password updated" });
        }

        [HttpPost("reset-password")]
        [EnableRateLimiting("reset")]
        public IActionResult ResetPassword([FromBody] ResetPasswordRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Token) || string.IsNullOrWhiteSpace(req.NewPassword))
                return BadRequest(new { message = "Token and password are required" });

            if (req.NewPassword.Length < 8)
                return BadRequest(new { message = "Password must be at least 8 characters" });

            using (var db = new UserContext())
            {
                var u = db.Users.FirstOrDefault(u => u.PasswordResetToken == req.Token);
                if (u == null || u.PasswordResetTokenExpiry == null || u.PasswordResetTokenExpiry < DateTime.UtcNow)
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
    public class ChangePasswordRequest { public string CurrentPassword { get; set; } = ""; public string NewPassword { get; set; } = ""; }
}
