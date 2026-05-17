using System.ComponentModel.DataAnnotations;

namespace ClickExpress.Domain.Models.Lead
{
    public class UpdateLeadStatusDTO
    {
        [Required][MaxLength(30)] public string Status { get; set; } = string.Empty;
    }
}
