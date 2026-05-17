using System.ComponentModel.DataAnnotations;

namespace ClickExpress.Domain.Models.Product
{
    public class UpdatePriceDTO
    {
        [Range(0.01, double.MaxValue)] public decimal Price { get; set; }
    }
}
