using System.ComponentModel.DataAnnotations;

namespace ClickExpress.Domain.Models.Review
{
    public class CreateReviewDTO
    {
        public int? ProductId { get; set; }
        [Range(1, 5)] public int Rating { get; set; }
        [Required] public string Text { get; set; } = string.Empty;
        public string? Role { get; set; }
        public string? Location { get; set; }
    }
}
