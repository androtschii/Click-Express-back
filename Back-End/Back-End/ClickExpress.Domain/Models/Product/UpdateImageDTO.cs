using System.ComponentModel.DataAnnotations;

namespace ClickExpress.Domain.Models.Product
{
    public class UpdateImageDTO
    {
        [Required] public string ImageUrl { get; set; } = string.Empty;
    }
}
