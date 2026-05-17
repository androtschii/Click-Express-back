using System.ComponentModel.DataAnnotations;

namespace ClickExpress.Domain.Models.JobApplication
{
    public class CreateJobApplicationDTO
    {
        [Required][MaxLength(100)] public string FullName { get; set; } = string.Empty;
        [Required][EmailAddress][MaxLength(150)] public string Email { get; set; } = string.Empty;
        [MaxLength(30)] public string Phone { get; set; } = string.Empty;
        [Required][MaxLength(100)] public string Position { get; set; } = string.Empty;
        [MaxLength(2000)] public string Message { get; set; } = string.Empty;
    }
}
