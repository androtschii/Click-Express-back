using System.ComponentModel.DataAnnotations;

namespace ClickExpress.Domain.Models.News
{
    public class CreateNewsDTO
    {
        [Required] public string Title { get; set; } = string.Empty;
        [Required] public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public bool IsPublished { get; set; } = true;
    }
}
