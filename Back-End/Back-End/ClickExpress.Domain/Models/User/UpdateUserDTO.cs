using System.ComponentModel.DataAnnotations;

namespace ClickExpress.Domain.Models.User
{
    public class UpdateUserDTO
    {
        [Required][MaxLength(50)] public string Username { get; set; } = string.Empty;
        [Required][EmailAddress][MaxLength(100)] public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        public bool IsActive { get; set; }
    }
}
