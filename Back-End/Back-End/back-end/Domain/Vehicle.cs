using System;

namespace back_end.Domain
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string Model { get; set; } = string.Empty;
        public string PlateNumber { get; set; } = string.Empty;
        public string Type { get; set; } = "Flatbed";
        public decimal Capacity { get; set; }
        public int Year { get; set; }
        public bool IsAvailable { get; set; } = true;
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
