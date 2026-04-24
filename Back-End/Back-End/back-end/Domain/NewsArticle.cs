using System;

namespace back_end.Domain
{
    public class NewsArticle
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int AuthorId { get; set; }
        public DateTime PublishedAt { get; set; } = DateTime.Now;
        public bool IsPublished { get; set; } = true;

        public User Author { get; set; } = null!;
    }
}
