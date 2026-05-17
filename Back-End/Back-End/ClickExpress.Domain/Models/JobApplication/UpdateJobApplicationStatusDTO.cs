using System.ComponentModel.DataAnnotations;

namespace ClickExpress.Domain.Models.JobApplication
{
    public class UpdateJobApplicationStatusDTO
    {
        [Required][MaxLength(30)] public string Status { get; set; } = string.Empty;
    }
}
