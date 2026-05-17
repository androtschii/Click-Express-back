using System.ComponentModel.DataAnnotations;

namespace ClickExpress.Domain.Models.User
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Company { get; set; }
        public string? Address { get; set; }
    }

    public class UserRegDTO
    {
        [Required] public string Username { get; set; } = string.Empty;
        [Required][EmailAddress] public string Email { get; set; } = string.Empty;
        [Required][MinLength(6)] public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
    }

    public class UserAuthDTO
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class UserActivateDTO
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateUserDTO
    {
        [Required][MaxLength(50)] public string Username { get; set; } = string.Empty;
        [Required][EmailAddress][MaxLength(100)] public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        public bool IsActive { get; set; }
    }

    public class UpdateProfileDTO
    {
        [MaxLength(100)] public string? FullName { get; set; }
        [Phone][MaxLength(30)] public string? Phone { get; set; }
        [MaxLength(100)] public string? Company { get; set; }
        [MaxLength(250)] public string? Address { get; set; }
        [EmailAddress][MaxLength(100)] public string? Email { get; set; }
    }
}
