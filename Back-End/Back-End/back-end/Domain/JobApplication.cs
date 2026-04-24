using System;

namespace back_end.Domain
{
    public class JobApplication
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Status { get; set; } = "New";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
