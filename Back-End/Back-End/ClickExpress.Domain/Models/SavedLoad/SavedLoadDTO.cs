using System.ComponentModel.DataAnnotations;

namespace ClickExpress.Domain.Models.SavedLoad
{
    public class SavedLoadDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal ProductPrice { get; set; }
        public string ProductImageUrl { get; set; } = string.Empty;
        public string ProductCategory { get; set; } = string.Empty;
        public string ProductDescription { get; set; } = string.Empty;
    }

    public class AddSavedLoadDTO
    {
        [Required] public int ProductId { get; set; }
    }
}
