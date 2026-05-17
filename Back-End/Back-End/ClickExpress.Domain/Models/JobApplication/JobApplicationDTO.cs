using System.ComponentModel.DataAnnotations;

namespace ClickExpress.Domain.Models.JobApplication
{
    public class JobApplicationDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CreateJobApplicationDTO
    {
        [Required][MaxLength(100)] public string FullName { get; set; } = string.Empty;
        [Required][EmailAddress][MaxLength(150)] public string Email { get; set; } = string.Empty;
        [MaxLength(30)] public string Phone { get; set; } = string.Empty;
        [Required][MaxLength(100)] public string Position { get; set; } = string.Empty;
        [MaxLength(2000)] public string Message { get; set; } = string.Empty;
    }

    public class UpdateJobApplicationStatusDTO
    {
        [Required][MaxLength(30)] public string Status { get; set; } = string.Empty;
    }
}
