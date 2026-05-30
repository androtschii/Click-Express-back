using ClickExpress.Domain.Entities.User;

namespace ClickExpress.Domain.Entities.News
{
    public class NewsArticleData
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int AuthorId { get; set; }
        public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
        public bool IsPublished { get; set; } = true;

        public UserData Author { get; set; } = null!;
    }
}
