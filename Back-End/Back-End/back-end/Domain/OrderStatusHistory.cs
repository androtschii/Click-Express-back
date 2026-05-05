namespace back_end.Domain
{
    public class OrderStatusHistory
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? Location { get; set; }
        public string? Note { get; set; }

        public Order Order { get; set; } = null!;
    }
}
