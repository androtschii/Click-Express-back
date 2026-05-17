using System.ComponentModel.DataAnnotations;

namespace ClickExpress.Domain.Models.News
{
    public class NewsArticleDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public DateTime PublishedAt { get; set; }
        public bool IsPublished { get; set; }
    }

    public class CreateNewsDTO
    {
        [Required] public string Title { get; set; } = string.Empty;
        [Required] public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public bool IsPublished { get; set; } = true;
    }
}
