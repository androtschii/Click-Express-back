using System.ComponentModel.DataAnnotations;

namespace ClickExpress.Domain.Models.Review
{
    public class ReviewDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public int? ProductId { get; set; }
        public int Rating { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsApproved { get; set; }
    }

    public class CreateReviewDTO
    {
        public int? ProductId { get; set; }
        [Range(1, 5)] public int Rating { get; set; }
        [Required] public string Text { get; set; } = string.Empty;
    }
}
