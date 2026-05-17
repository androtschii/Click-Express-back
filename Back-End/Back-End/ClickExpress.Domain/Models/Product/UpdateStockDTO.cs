using System.ComponentModel.DataAnnotations;

namespace ClickExpress.Domain.Models.Product
{
    public class UpdateStockDTO
    {
        [Range(0, int.MaxValue)] public int Quantity { get; set; }
    }
}
