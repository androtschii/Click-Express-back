using System.ComponentModel.DataAnnotations;

namespace ClickExpress.Domain.Models.User
{
    public class UpdateProfileDTO
    {
        [MaxLength(100)] public string? FullName { get; set; }
        [Phone][MaxLength(30)] public string? Phone { get; set; }
        [MaxLength(100)] public string? Company { get; set; }
        [MaxLength(250)] public string? Address { get; set; }
        [EmailAddress][MaxLength(100)] public string? Email { get; set; }
    }
}
