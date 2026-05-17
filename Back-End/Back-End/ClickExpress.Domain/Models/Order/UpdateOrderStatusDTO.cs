using System.ComponentModel.DataAnnotations;

namespace ClickExpress.Domain.Models.Order
{
    public class UpdateOrderStatusDTO
    {
        [Required] public string Status { get; set; } = string.Empty;
    }
}
