using System.ComponentModel.DataAnnotations;

namespace ClickExpress.Domain.Models.Notification
{
    public class CreateNotificationDTO
    {
        public int? UserId { get; set; }
        [Required] public string Title { get; set; } = string.Empty;
        [Required] public string Body { get; set; } = string.Empty;
        public string Type { get; set; } = "info";
    }
}
