using System;

namespace back_end.Domain
{
    public class Lead
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Company { get; set; }
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public string Equipment { get; set; } = string.Empty;
        public decimal? Weight { get; set; }
        public DateTime? PickupDate { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Status { get; set; } = "New";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
