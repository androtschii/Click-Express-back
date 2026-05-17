using System.ComponentModel.DataAnnotations;

namespace ClickExpress.Domain.Models.Product
{
    public class UpdateProductDTO
    {
        [Required][MaxLength(100)] public string Name { get; set; } = string.Empty;
        [MaxLength(500)] public string Description { get; set; } = string.Empty;
        [Range(0.01, double.MaxValue)] public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        [Required] public string Category { get; set; } = string.Empty;
        [Range(0, int.MaxValue)] public int Stock { get; set; }
        public bool IsActive { get; set; }
    }
}
