using ClickExpress.Domain.Entities.Order;
using ClickExpress.Domain.Entities.User;

namespace ClickExpress.Domain.Entities.Document
{
    public class DocumentData
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public int? OrderId { get; set; }
        public int UploadedBy { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public OrderData? Order { get; set; }
        public UserData UploadedByUser { get; set; } = null!;
    }
}
