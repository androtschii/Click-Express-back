using ClickExpress.Domain.Entities.User;
using ClickExpress.Domain.Entities.Product;

namespace ClickExpress.Domain.Entities.Review
{
    public class ReviewData
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? ProductId { get; set; }
        public int Rating { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsApproved { get; set; } = false;
        public string? Role { get; set; }
        public string? Location { get; set; }

        public UserData User { get; set; } = null!;
        public ProductData? Product { get; set; }
    }
}
