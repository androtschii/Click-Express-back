namespace back_end.Domain
{
    public class Document
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public int? OrderId { get; set; }
        public int UploadedBy { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public Order? Order { get; set; }
        public User UploadedByUser { get; set; } = null!;
    }
}
