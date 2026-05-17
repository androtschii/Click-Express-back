using System.ComponentModel.DataAnnotations;

namespace ClickExpress.Domain.Models.Driver
{
    public class PatchDriverStatusDTO
    {
        [Required][MaxLength(50)] public string Status { get; set; } = string.Empty;
    }
}
