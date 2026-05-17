namespace ClickExpress.Domain.Models.Order
{
    public class OrderStatusHistoryDTO
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string? Location { get; set; }
        public string? Note { get; set; }
    }
}
