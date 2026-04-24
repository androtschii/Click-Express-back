using System;

namespace back_end.Domain
{
    public class Review
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? ProductId { get; set; }
        public int Rating { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsApproved { get; set; } = false;

        public User User { get; set; } = null!;
        public Product? Product { get; set; }
    }
}
